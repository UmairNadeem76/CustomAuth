using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomAuth.Entitites;
using System.Linq;

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

			return Ok(users);
		}

		[HttpGet("users/{userId}/tasks")]
		public IActionResult GetUserTasks(int userId)
		{
			// Verify that the user exists
			var userExists = _context.UserAccounts.Any(u => u.ID == userId);
			if (!userExists)
			{
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

			return Ok(tasks);
		}
	}
}
