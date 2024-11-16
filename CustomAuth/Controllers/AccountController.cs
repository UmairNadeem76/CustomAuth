// AccountController.cs

// Importing required namespaces for functionalities such as JWT, database interactions, logging, and authentication
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomAuth.Entitites; // Custom entity definitions
using CustomAuth.Models; // Custom models for handling data
using CustomAuth.Services; // Custom services, including password hashing
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // For securing endpoints
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For database interactions
using Microsoft.IdentityModel.Tokens; // For JWT generation and validation
using Serilog; // For logging

namespace CustomAuth.Controllers
{
	// Route setup for the controller and API convention
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		// Dependency injection for database context and password service
		private readonly AppDbContext _context; // Database context
		private readonly PasswordService _passwordService; // Service for password hashing and verification

		// Constructor to initialize the database context and password service
		public AccountController(AppDbContext appDbContext)
		{
			_context = appDbContext;
			_passwordService = new PasswordService();
		}

		// GET api/account: Retrieves all user accounts, excluding sensitive data
		[HttpGet]
		public IActionResult GetUsers()
		{
			Log.Information("GET api/account called to retrieve all users.");
			var users = _context.UserAccounts.Select(u => new
			{
				u.ID,
				u.Email,
				u.FirstName,
				u.LastName,
				u.UserName,
				// Excluding sensitive information like passwords
			}).ToList();
			Log.Information("Returned {UserCount} users.", users.Count);
			return Ok(users);
		}

		// POST api/account/register: Registers a new user
		[HttpPost("register")]
		public IActionResult Register([FromBody] RegistrationViewModel model)
		{
			Log.Information("Registration attempt for email {Email}", model.Email);
			if (ModelState.IsValid)
			{
				// Hash the user password before saving
				var hashedpassword = _passwordService.HashPassword(model.Password);
				var account = new UserAccount
				{
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Password = hashedpassword, // Store hashed password
					UserName = model.UserName
				};

				try
				{
					// Save the new user to the database
					_context.UserAccounts.Add(account);
					_context.SaveChanges();
					Log.Information("User {FirstName} {LastName} registered successfully.", account.FirstName, account.LastName);
					return Ok(new { Message = $"{account.FirstName} {account.LastName} Registered Successfully. Please Login." });
				}
				catch (DbUpdateException ex)
				{
					// Handle duplicate entries or database update errors
					Log.Error(ex, "Registration failed for email {Email}.", model.Email);
					return BadRequest("Please Enter Unique Email Or Password");
				}
			}
			Log.Warning("Registration failed due to invalid model state for email {Email}", model.Email);
			return BadRequest(ModelState);
		}

		// POST api/account/login: Authenticates a user and issues a JWT
		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginViewModel model)
		{
			Log.Information("Login attempt for {UserNameOrEmail}", model.UserNameOrEmail);
			if (ModelState.IsValid)
			{
				// Retrieve user based on email or username
				var user = _context.UserAccounts
					.FirstOrDefault(x => (x.UserName == model.UserNameOrEmail || x.Email == model.UserNameOrEmail));

				// Verify the provided password
				bool isValid = user != null && _passwordService.VerifyPassword(model.Password, user.Password);

				if (isValid)
				{
					// Determine if the user is an admin
					bool isAdmin = user.ID == 1;

					Log.Information("User {Email} authenticated successfully. IsAdmin: {IsAdmin}", user.Email, isAdmin);

					// Generate a JWT for the authenticated user
					var tokenHandler = new JwtSecurityTokenHandler();
					var key = Encoding.ASCII.GetBytes("8DpE3PAHFEhVg5uOCzpqIDrxTy18XD9eJ+++XVyrbXAzYAIEpNltoUjEA3f+5G9X"); // Secret key
					var tokenDescriptor = new SecurityTokenDescriptor
					{
						Subject = new ClaimsIdentity(new[]
						{
							new Claim(ClaimTypes.Name, user.Email),
							new Claim("UserID", user.ID.ToString()),
							new Claim("IsAdmin", isAdmin.ToString()) // Custom claim for admin users
                        }),
						Expires = DateTime.UtcNow.AddHours(1), // Token expiration
						SigningCredentials = new SigningCredentials(
							new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
					};
					var token = tokenHandler.CreateToken(tokenDescriptor);
					var tokenString = tokenHandler.WriteToken(token);

					// Store the JWT as a secure HTTP-only cookie
					var cookieOptions = new CookieOptions
					{
						HttpOnly = true,
						Secure = true,
						Expires = DateTime.UtcNow.AddHours(1)
					};
					Response.Cookies.Append("jwt", tokenString, cookieOptions);

					Log.Information("JWT token generated and stored for user {Email}", user.Email);
					return Ok(new { Message = "Login Successful" });
				}
				else
				{
					Log.Warning("Login failed for {UserNameOrEmail}: Invalid credentials.", model.UserNameOrEmail);
					return Unauthorized("Username/Email or Password Is Incorrect");
				}
			}
			Log.Warning("Login failed due to invalid model state for {UserNameOrEmail}", model.UserNameOrEmail);
			return BadRequest(ModelState);
		}

		// GET api/account/userdata: Returns authenticated user's data
		[Authorize] // Endpoint requires authentication
		[HttpGet("userdata")]
		public IActionResult GetUserData()
		{
			Log.Information("Authenticated user {User} is requesting userdata.", User.Identity?.Name);

			// Retrieve UserID from JWT claims
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null)
			{
				Log.Warning("User data requested without a valid UserID claim.");
				return Unauthorized("User not authenticated");
			}

			if (!int.TryParse(userIdClaim, out int userId))
			{
				Log.Warning("Invalid UserID claim value: {UserID}", userIdClaim);
				return Unauthorized("Invalid user ID");
			}

			// Fetch user details from the database
			var user = _context.UserAccounts.FirstOrDefault(u => u.ID == userId);
			if (user == null)
			{
				Log.Warning("User data requested for non-existent UserID: {UserID}", userId);
				return NotFound("User not found");
			}

			user.Password = null; // Exclude sensitive information

			Log.Information("User data retrieved successfully for UserID: {UserID}", userId);
			return Ok(new
			{
				user.ID,
				user.Email,
				user.FirstName,
				user.LastName,
				user.UserName,
			});
		}

		// POST api/account/logout: Logs out the user by deleting the JWT cookie
		[HttpPost("logout")]
		public IActionResult Logout()
		{
			var email = User.Identity?.Name;
			if (!string.IsNullOrEmpty(email))
			{
				Log.Information("User {Email} is logging out.", email);
			}
			else
			{
				Log.Information("Anonymous user is attempting to log out.");
			}

			// Remove the JWT cookie
			Response.Cookies.Delete("jwt", new CookieOptions
			{
				HttpOnly = true,
				Secure = true, // Matches the original cookie settings
				SameSite = SameSiteMode.Strict,
				Path = "/"
			});

			Log.Information("JWT cookie deleted for user {Email}. Logout successful.", email);
			return Ok(new { Message = "Logout Successful" });
		}
	}
}
