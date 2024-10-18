using System.Security.Claims;
using CustomAuth.Entitites;
using CustomAuth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, user.Email),
						new Claim("Name", user.FirstName),
						new Claim(ClaimTypes.Role, "User")
					};

					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

					return Ok(new { Message = "Login Successful" });
				}
				else
				{
					return Unauthorized("Username/Email Or Password Is Not Correct");
				}
			}
			return BadRequest(ModelState);
		}
		}
	}