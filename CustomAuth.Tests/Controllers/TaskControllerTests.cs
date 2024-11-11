using Xunit;
using CustomAuth.Controllers;
using CustomAuth.Entitites;
using CustomAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CustomAuth.Tests.Controllers
{
	public class TaskControllerTests
	{
		private TaskController _controller;
		private AppDbContext _context;

		private void InitializeTest()
		{
			// Generate a unique database name for each test
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new AppDbContext(options);
			SeedDatabase();

			_controller = new TaskController(_context);

			// Mock authenticated user with ID = 1
			var user = _context.UserAccounts.First(u => u.ID == 1);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
				new Claim(ClaimTypes.Name, user.Email)
			};
			var identity = new ClaimsIdentity(claims, "TestAuthType");
			var claimsPrincipal = new ClaimsPrincipal(identity);

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};
		}

		private void SeedDatabase()
		{
			_context.UserAccounts.Add(
				new UserAccount
				{
					ID = 1,
					Email = "user1@example.com",
					FirstName = "User",
					LastName = "One",
					Password = "UserPass123",
					UserName = "userone"
				}
			);
			_context.SaveChanges();
		}

		[Fact]
		public async Task CreateTask_ValidTask_ReturnsOk()
		{
			InitializeTest();

			// Arrange
			var newTaskDto = new TaskCreateDto
			{
				Task_Name = "New Task",
				Task_Description = "Task Description",
				Task_Status = "Open",
				Task_Priority = 1
			};

			// Act
			var result = await _controller.CreateTask(newTaskDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(new { Message = "Task Created Successfully" }.ToString(), okResult.Value.ToString());

			// Verify that the task was added
			var task = _context.TaskList.FirstOrDefault(t => t.Task_Name == newTaskDto.Task_Name);
			Assert.NotNull(task);
			Assert.Equal(newTaskDto.Task_Description, task.Task_Description);
		}
	}
}
