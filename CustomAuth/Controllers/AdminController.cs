// AdminController.cs

// Importing required namespaces for managing API requests, database operations, and logging
using Microsoft.AspNetCore.Authorization; // For applying authorization policies
using Microsoft.AspNetCore.Mvc; // For defining controllers and handling HTTP requests
using CustomAuth.Entitites; // Custom entity definitions
using System.Linq; // For LINQ queries
using Serilog; // For structured logging

namespace CustomAuth.Controllers
{
	// Setting up the API route and applying admin-only authorization policy
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "AdminOnly")] // Restricts access to users with the "AdminOnly" policy
	public class AdminController : ControllerBase
	{
		// Dependency injection for the database context
		private readonly AppDbContext _context;

		// Constructor to initialize the database context
		public AdminController(AppDbContext context)
		{
			_context = context;
		}

		// GET api/admin/users: Retrieves a list of all users
		[HttpGet("users")]
		public IActionResult GetAllUsers()
		{
			// Retrieve the admin's email from the authenticated user's identity
			var adminEmail = User.Identity?.Name ?? "Unknown Admin";
			Log.Information("Admin {AdminEmail} requested all users.", adminEmail);

			// Fetch user details from the database, excluding sensitive fields like passwords
			var users = _context.UserAccounts
				.Select(u => new
				{
					u.ID,
					u.FirstName,
					u.LastName,
					u.Email,
					u.UserName
				})
				.ToList();

			Log.Information("Admin {AdminEmail} retrieved {UserCount} users.", adminEmail, users.Count);

			// Return the list of users
			return Ok(users);
		}

		// GET api/admin/users/{userId}/tasks: Retrieves tasks assigned to a specific user
		[HttpGet("users/{userId}/tasks")]
		public IActionResult GetUserTasks(int userId)
		{
			// Retrieve the admin's email from the authenticated user's identity
			var adminEmail = User.Identity?.Name ?? "Unknown Admin";
			Log.Information("Admin {AdminEmail} requested tasks for UserID {UserId}.", adminEmail, userId);

			// Check if the specified user exists in the database
			var userExists = _context.UserAccounts.Any(u => u.ID == userId);
			if (!userExists)
			{
				Log.Warning("Admin {AdminEmail} requested tasks for non-existent UserID {UserId}.", adminEmail, userId);
				return NotFound(new { Message = $"User with ID {userId} not found." });
			}

			// Fetch tasks assigned to the specified user, excluding sensitive fields
			var tasks = _context.TaskList
				.Where(t => t.UserID == userId)
				.Select(t => new
				{
					t.TaskID,
					t.Task_Name,
					t.Task_Description,
					t.Task_Status,
					t.Task_Priority
				})
				.ToList();

			Log.Information("Admin {AdminEmail} retrieved {TaskCount} tasks for UserID {UserId}.", adminEmail, tasks.Count, userId);

			// Return the list of tasks
			return Ok(tasks);
		}
	}
}
