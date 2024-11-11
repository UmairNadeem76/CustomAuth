// AccountController.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomAuth.Entitites;
using CustomAuth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog; // Add Serilog

namespace CustomAuth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly AppDbContext _context;

		public AccountController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}

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
				// Exclude Password
			}).ToList();
			Log.Information("Returned {UserCount} users.", users.Count);
			return Ok(users);
		}

		[HttpPost("register")]
		public IActionResult Register([FromBody] RegistrationViewModel model)
		{
			Log.Information("Registration attempt for email {Email}", model.Email);
			if (ModelState.IsValid)
			{
				var account = new UserAccount
				{
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Password = model.Password,
					UserName = model.UserName
				};

				try
				{
					_context.UserAccounts.Add(account);
					_context.SaveChanges();
					Log.Information("User {FirstName} {LastName} registered successfully.", account.FirstName, account.LastName);
					return Ok(new { Message = $"{account.FirstName} {account.LastName} Registered Successfully. Please Login." });
				}
				catch (DbUpdateException ex)
				{
					Log.Error(ex, "Registration failed for email {Email}.", model.Email);
					return BadRequest("Please Enter Unique Email Or Password");
				}
			}
			Log.Warning("Registration failed due to invalid model state for email {Email}", model.Email);
			return BadRequest(ModelState);
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginViewModel model)
		{
			Log.Information("Login attempt for {UserNameOrEmail}", model.UserNameOrEmail);
			if (ModelState.IsValid)
			{
				var user = _context.UserAccounts
					.FirstOrDefault(x => (x.UserName == model.UserNameOrEmail || x.Email == model.UserNameOrEmail) &&
										  x.Password == model.Password);

				if (user != null)
				{
					// Determine if the user is admin (UserID == 9)
					bool isAdmin = user.ID == 9;

					Log.Information("User {Email} authenticated successfully. IsAdmin: {IsAdmin}", user.Email, isAdmin);

					// Generate JWT token
					var tokenHandler = new JwtSecurityTokenHandler();
					var key = Encoding.ASCII.GetBytes("8DpE3PAHFEhVg5uOCzpqIDrxTy18XD9eJ+++XVyrbXAzYAIEpNltoUjEA3f+5G9X"); // Use a secure secret key
					var tokenDescriptor = new SecurityTokenDescriptor
					{
						Subject = new ClaimsIdentity(new[]
						{
							new Claim(ClaimTypes.Name, user.Email),
							new Claim("UserID", user.ID.ToString()),
							new Claim("IsAdmin", isAdmin.ToString()) // Add IsAdmin claim
                        }),
						Expires = DateTime.UtcNow.AddHours(1),
						SigningCredentials = new SigningCredentials(
							new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
					};
					var token = tokenHandler.CreateToken(tokenDescriptor);
					var tokenString = tokenHandler.WriteToken(token);

					// Store the JWT in a cookie
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

		[Authorize]
		[HttpGet("userdata")]
		public IActionResult GetUserData()
		{
			Log.Information("Authenticated user {User} is requesting userdata.", User.Identity?.Name);
			// Extract the UserID from the claims
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null)
			{
				Log.Warning("User data requested without a valid UserID claim.");
				return Unauthorized("User not authenticated");
			}

			// Parse the UserID to an integer
			if (!int.TryParse(userIdClaim, out int userId))
			{
				Log.Warning("Invalid UserID claim value: {UserID}", userIdClaim);
				return Unauthorized("Invalid user ID");
			}

			// Query the database for the user
			var user = _context.UserAccounts.FirstOrDefault(u => u.ID == userId);
			if (user == null)
			{
				Log.Warning("User data requested for non-existent UserID: {UserID}", userId);
				return NotFound("User not found");
			}

			// Exclude sensitive data before returning
			user.Password = null; // Remove the password

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

			// Delete the JWT cookie by setting its expiration date to the past
			Response.Cookies.Delete("jwt", new CookieOptions
			{
				HttpOnly = true,
				Secure = true, // Ensure this matches how the cookie was originally set
				SameSite = SameSiteMode.Strict,
				Path = "/"
			});

			Log.Information("JWT cookie deleted for user {Email}. Logout successful.", email);

			return Ok(new { Message = "Logout Successful" });
		}
	}
}
