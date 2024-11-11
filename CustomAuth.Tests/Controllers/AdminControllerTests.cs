using Xunit;
using CustomAuth.Controllers;
using CustomAuth.Entitites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CustomAuth.Tests.Controllers
{
	public class AdminControllerTests
	{
		private AdminController _controller;
		private AppDbContext _context;

		private void InitializeTest(bool isAdmin = true)
		{
			// Generate a unique database name for each test
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new AppDbContext(options);
			SeedDatabase();

			_controller = new AdminController(_context);

			// Mock the user (admin or non-admin)
			var userAccount = _context.UserAccounts.First(u => u.ID == (isAdmin ? 9 : 1));
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userAccount.ID.ToString()),
				new Claim(ClaimTypes.Name, userAccount.Email),
				new Claim("IsAdmin", isAdmin.ToString())
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
			// Seed users
			_context.UserAccounts.AddRange(
				new UserAccount
				{
					ID = 9,
					Email = "admin@example.com",
					FirstName = "Admin",
					LastName = "User",
					Password = "AdminPass123",
					UserName = "adminuser"
				},
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

			// Seed tasks
			_context.TaskList.AddRange(
				new TaskList
				{
					TaskID = 1,
					Task_Name = "Task1",
					Task_Description = "Description1",
					Task_Status = "Open",
					Task_Priority = 1,
					UserID = 1
				},
				new TaskList
				{
					TaskID = 2,
					Task_Name = "Task2",
					Task_Description = "Description2",
					Task_Status = "Closed",
					Task_Priority = 2,
					UserID = 1
				}
			);

			_context.SaveChanges();
		}

		[Fact]
		public void GetAllUsers_AdminUser_ReturnsUsers()
		{
			InitializeTest(isAdmin: true);

			// Act
			var result = _controller.GetAllUsers();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var users = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

			Assert.Equal(2, users.Count()); // Admin and one user
		}
	}
}
