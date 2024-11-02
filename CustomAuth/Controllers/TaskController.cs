// TaskController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CustomAuth.Entitites;
using System.Diagnostics;
using System.Security.Claims;
using CustomAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protect all actions in this controller
public class TaskController : ControllerBase
{
	private readonly AppDbContext _context;

	public TaskController(AppDbContext context)
	{
		_context = context;
	}

	// 1. Create a new task
	[HttpPost("create")]
	public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto newTaskDto)
	{
		var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		if (userIdClaim == null)
		{
			return Unauthorized("UserID not found in the token.");
		}

		// Map DTO to entity
		var newTask = new TaskList
		{
			Task_Name = newTaskDto.Task_Name,
			Task_Description = newTaskDto.Task_Description,
			Task_Status = newTaskDto.Task_Status,
			Task_Priority = newTaskDto.Task_Priority,
			UserID = int.Parse(userIdClaim.Value)
		};

		_context.TaskList.Add(newTask);
		await _context.SaveChangesAsync();

		return Ok(new { Message = "Task Created Successfully" });
	}

	// 2. Update a task (name, description, status, priority)
	[HttpPut("update/{taskId}")]
	public async Task<IActionResult> UpdateTask(int taskId, [FromBody] Dictionary<string, object> updatedFields)
	{
		// Get the UserID of the authenticated user
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

		// Check if the task belongs to the authenticated user
		if (task.UserID != userId)
		{
			return Forbid("You do not have permission to update this task.");
		}

		// Update the task properties based on the provided fields
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
					// Ignore unknown fields or handle as needed
					break;
			}
		}

		// Save changes to the database
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

		// Delete the task
		_context.TaskList.Remove(task);
		await _context.SaveChangesAsync();

		return Ok(new { Message = "Task Deleted Successfully." });
	}

	[HttpGet("usertasks")]
	public async Task<IActionResult> GetUserTasks()
	{
		// Extract the UserID from the claims
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

		// Fetch tasks associated with the user
		var userTasks = await _context.TaskList
			.Where(t => t.UserID == userId)
			.ToListAsync();

		return Ok(userTasks);
	}
}