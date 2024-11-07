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
			var users = _context.UserAccounts.ToList();
			return Ok(users);
		}

		[HttpPost("register")]
		public IActionResult Register([FromBody] RegistrationViewModel model)
		{
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

					return Ok(new { Message = $"{account.FirstName} {account.LastName} Registered Successfully. Please Login." });
				}
				catch (DbUpdateException)
				{
					return BadRequest("Please Enter Unique Email Or Password");
				}
			}
			return BadRequest(ModelState);
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = _context.UserAccounts
					.FirstOrDefault(x => (x.UserName == model.UserNameOrEmail || x.Email == model.UserNameOrEmail) &&
										  x.Password == model.Password);

				if (user != null)
				{
					// Determine if the user is admin (UserID == 9)
					bool isAdmin = user.ID == 9;

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

					return Ok(new { Message = "Login Successful" });
				}
				else
				{
					return Unauthorized("Username/Email or Password Is Incorrect");
				}
			}
			return BadRequest(ModelState);
		}

		[Authorize]
		[HttpGet("userdata")]
		public IActionResult GetUserData()
		{
			// Extract the UserID from the claims
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null)
			{
				return Unauthorized("User not authenticated");
			}

			// Parse the UserID to an integer
			if (!int.TryParse(userIdClaim, out int userId))
			{
				return Unauthorized("Invalid user ID");
			}

			// Query the database for the user
			var user = _context.UserAccounts.FirstOrDefault(u => u.ID == userId);
			if (user == null)
			{
				return NotFound("User not found");
			}

			// Exclude sensitive data before returning
			user.Password = null; // Remove the password

			return Ok(user);
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{
			// Delete the JWT cookie by setting its expiration date to the past
			Response.Cookies.Delete("jwt", new CookieOptions
			{
				HttpOnly = true,
				Secure = true, // Ensure this matches how the cookie was originally set
				SameSite = SameSiteMode.Strict,
				Path = "/"
			});

			return Ok(new { Message = "Logout Successful" });
		}
	}
}