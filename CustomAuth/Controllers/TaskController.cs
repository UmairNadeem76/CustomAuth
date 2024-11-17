// TaskController.cs

// Importing required namespaces for managing API requests, authorization, database interactions, and other functionalities
using Microsoft.AspNetCore.Authorization; // To secure endpoints with authorization policies
using Microsoft.AspNetCore.Mvc; // To define the controller and handle HTTP requests
using System.Linq; // For LINQ queries
using System.Threading.Tasks; // For asynchronous programming
using System.Collections.Generic; // To handle collections like Dictionary
using CustomAuth.Entitites; // Custom-defined entities for database operations
using System.Diagnostics; // (Unused in this file but could be removed if not needed)
using System.Security.Claims; // For extracting user claims from authentication tokens
using CustomAuth.Models; // For data transfer objects (DTOs)
using Microsoft.EntityFrameworkCore; // For database interaction and queries
using Microsoft.IdentityModel.Tokens; // (Unused in this file but could be removed if not needed)

// Setting up the API route and applying authorization to secure the controller
[ApiController]
[Route("api/[controller]")]
[Authorize] // Ensures all actions are accessible only to authenticated users
public class TaskController : ControllerBase
{
	// Dependency injection for the database context
	private readonly AppDbContext _context;

	// Constructor to initialize the database context
	public TaskController(AppDbContext context)
	{
		_context = context;
	}

	// 1. Create a new task
	[HttpPost("create")]
	public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto newTaskDto)
	{
		// Extract the UserID from the authenticated user's claims
		var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		if (userIdClaim == null)
		{
			return Unauthorized("UserID not found in the token.");
		}

		// Map the data from the DTO to the TaskList entity
		var newTask = new TaskList
		{
			Task_Name = newTaskDto.Task_Name,
			Task_Description = newTaskDto.Task_Description,
			Task_Status = newTaskDto.Task_Status,
			Task_Priority = newTaskDto.Task_Priority,
			UserID = int.Parse(userIdClaim.Value) // Associate the task with the authenticated user
		};

		// Add the new task to the database and save changes asynchronously
		_context.TaskList.Add(newTask);
		await _context.SaveChangesAsync();

		return Ok(new { Message = "Task Created Successfully" });
	}

	// 2. Update an existing task
	[HttpPut("update/{taskId}")]
	public async Task<IActionResult> UpdateTask(int taskId, [FromBody] Dictionary<string, object> updatedFields)
	{
		// Extract the UserID from the authenticated user's claims
		var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
		{
			return Unauthorized("UserID not found in the token.");
		}

		// Find the task with the given taskId
		var task = await _context.TaskList.FirstOrDefaultAsync(t => t.TaskID == taskId);

		if (task == null)
		{
			return NotFound("Task not found.");
		}

		// Ensure the task belongs to the authenticated user
		if (task.UserID != userId)
		{
			return Forbid("You do not have permission to update this task.");
		}

		// Update task properties based on the provided fields
		foreach (var field in updatedFields)
		{
			switch (field.Key.ToLower())
			{
				case "task_name":
					task.Task_Name = field.Value.ToString();
					break;
				case "task_description":
					task.Task_Description = field.Value.ToString();
					break;
				case "task_status":
					task.Task_Status = field.Value.ToString();
					break;
				case "task_priority":
					if (int.TryParse(field.Value.ToString(), out int priority))
					{
						task.Task_Priority = priority;
					}
					else
					{
						return BadRequest("Invalid value for Task_Priority.");
					}
					break;
				default:
					// Ignore unknown fields
					break;
			}
		}

		// Save changes to the database asynchronously
		await _context.SaveChangesAsync();

		return Ok(new { Message = "Task updated successfully." });
	}

	// 3. Delete a task
	[HttpDelete("delete/{taskId}")]
	public async Task<IActionResult> DeleteTask(int taskId)
	{
		// Find the task with the given taskId
		var task = await _context.TaskList.FirstOrDefaultAsync(t => t.TaskID == taskId);

		if (task == null)
		{
			return NotFound("Task Not Found.");
		}

		// Soft delete the task by setting its isDeleted flag to true
		task.isDeleted = true;
		await _context.SaveChangesAsync();

		return Ok(new { Message = "Task Deleted Successfully." });
	}

	// 4. Retrieve all tasks for the authenticated user
	[HttpGet("usertasks")]
	public async Task<IActionResult> GetUserTasks()
	{
		// Extract the UserID from the authenticated user's claims
		var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		if (userIdClaim == null)
		{
			return Unauthorized("UserID not found in the token.");
		}

		// Parse the UserID to an integer
		if (!int.TryParse(userIdClaim.Value, out int userId))
		{
			return Unauthorized("Invalid user ID");
		}

		// Fetch tasks that belong to the user and are not deleted
		var userTasks = await _context.TaskList
			.Where(t => t.UserID == userId && !t.isDeleted)
			.ToListAsync();

		return Ok(userTasks);
	}

	// API for Server Side Filtering
	[HttpGet("usertask/{taskstatus}")]
	public async Task<IActionResult> GetByTaskStatus(string taskstatus)
	{
		string Task_Status = taskstatus;
		if (Task_Status == "InProgress")
		{
			Task_Status = "In Progress";
		}

		// Extract the UserID from the authenticated user's claims
		var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		if (userIdClaim == null)
		{
			// Return 401 Unauthorized if the UserID claim is missing
			return Unauthorized("UserID not found in the token.");
		}

		// Parse the UserID claim to an integer
		if (!int.TryParse(userIdClaim.Value, out int userId))
		{
			// Return 401 Unauthorized if the UserID claim is invalid
			return Unauthorized("Invalid user ID");
		}

		// Fetch tasks from the database that:
		// - Belong to the authenticated user
		// - Are not soft-deleted (isDeleted = false)
		// - Match the provided Task_Status
		var userTasks = await _context.TaskList
			.Where(t => t.UserID == userId && !t.isDeleted && t.Task_Status == Task_Status)
			.ToListAsync();

		// Return the filtered tasks as an HTTP 200 OK response
		return Ok(userTasks);
	}

}
