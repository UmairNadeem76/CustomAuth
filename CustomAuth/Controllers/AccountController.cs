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
					// Generate JWT token
					var tokenHandler = new JwtSecurityTokenHandler();
					var key = Encoding.ASCII.GetBytes("6567e69b55419b2c16b904595c5af62257603b577d1da3b0b6c60375ebbc7bf7bca9ab475b7c6782b7850e3e0311ed32cb0e4d0be9a2f2797ba5d7511e577293da09aa1a3d1cd261bf500d4efd65166cdbb9a752ffabe1c47b970e6336dec0ea24afd3e71bde5e2d0b9fddba56c09870f23ac67926b5e3f15b7b7b856fb3d751ffc7dca579d62c77f530006db3144400a99cf0f53c959a78eb6a2e5a391fca712b61708897bcd072e21524d577105fe3a1bffd658da6ec1cc13a32139713ed0322beb1ef38dbf03f86116ad600bf30e59785ed79f447307a76ab29bd9e504b71be8dead2d1466173e76d362e0c2d8b6c9b34b93040b6de39408469faabee9673"); // Use a secure and stored secret key
					var tokenDescriptor = new SecurityTokenDescriptor
					{
						Subject = new ClaimsIdentity(new Claim[]
						{
					new Claim(ClaimTypes.Name, user.Email),
					new Claim("UserID", user.ID.ToString())
						}),
						Expires = DateTime.UtcNow.AddHours(1),
						SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
					};
					var token = tokenHandler.CreateToken(tokenDescriptor);
					var tokenString = tokenHandler.WriteToken(token);

					// Store the JWT in a cookie
					var cookieOptions = new CookieOptions
					{
						HttpOnly = true, // Prevent access to the cookie via client-side scripts
						Secure = true,   // Ensure the cookie is only sent over HTTPS
						Expires = DateTime.UtcNow.AddHours(1) // Set cookie expiration to match token expiration
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
	}
}