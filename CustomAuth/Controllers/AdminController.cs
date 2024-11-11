// AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomAuth.Entitites;
using System.Linq;
using Serilog; // Add Serilog

namespace CustomAuth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "AdminOnly")] // Apply admin-only policy
	public class AdminController : ControllerBase
	{
		private readonly AppDbContext _context;

		public AdminController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("users")]
		public IActionResult GetAllUsers()
		{
			var adminEmail = User.Identity?.Name ?? "Unknown Admin";
			Log.Information("Admin {AdminEmail} requested all users.", adminEmail);

			var users = _context.UserAccounts
				.Select(u => new
				{
					u.ID,
					u.FirstName,
					u.LastName,
					u.Email,
					u.UserName
					// Exclude Password and other sensitive fields
				})
				.ToList();

			Log.Information("Admin {AdminEmail} retrieved {UserCount} users.", adminEmail, users.Count);

			return Ok(users);
		}

		[HttpGet("users/{userId}/tasks")]
		public IActionResult GetUserTasks(int userId)
		{
			var adminEmail = User.Identity?.Name ?? "Unknown Admin";
			Log.Information("Admin {AdminEmail} requested tasks for UserID {UserId}.", adminEmail, userId);

			// Verify that the user exists
			var userExists = _context.UserAccounts.Any(u => u.ID == userId);
			if (!userExists)
			{
				Log.Warning("Admin {AdminEmail} requested tasks for non-existent UserID {UserId}.", adminEmail, userId);
				return NotFound(new { Message = $"User with ID {userId} not found." });
			}

			// Fetch tasks for the specified user
			var tasks = _context.TaskList
				.Where(t => t.UserID == userId)
				.Select(t => new
				{
					t.TaskID,
					t.Task_Name,
					t.Task_Description,
					t.Task_Status,
					t.Task_Priority
					// Exclude any sensitive fields
				})
				.ToList();

			Log.Information("Admin {AdminEmail} retrieved {TaskCount} tasks for UserID {UserId}.", adminEmail, tasks.Count, userId);

			return Ok(tasks);
		}
	}
}
