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

namespace CustomAuth.Tests.Controllers
{
	public class AccountControllerTests
	{
		private AccountController _controller;
		private AppDbContext _context;

		private void InitializeTest()
		{
			// Generate a unique database name for each test
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new AppDbContext(options);

			// Seed the database
			SeedDatabase();

			_controller = new AccountController(_context);
		}

		private void SeedDatabase()
		{
			// Add a user with ID = 9 to represent the admin
			_context.UserAccounts.Add(new UserAccount
			{
				ID = 9,
				Email = "admin@example.com",
				FirstName = "Admin",
				LastName = "User",
				Password = "AdminPass123",
				UserName = "adminuser"
			});

			_context.SaveChanges();
		}

		[Fact]
		public void Register_ValidUser_ReturnsOk()
		{
			InitializeTest();

			// Arrange
			var model = new RegistrationViewModel
			{
				Email = "testuser@example.com",
				FirstName = "Test",
				LastName = "User",
				Password = "Password123",
				UserName = "testuser"
			};

			// Act
			var result = _controller.Register(model);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);

			// Verify that the user was added to the database
			var user = _context.UserAccounts.FirstOrDefault(u => u.Email == model.Email);
			Assert.NotNull(user);
			Assert.Equal(model.FirstName, user.FirstName);
		}

		// ... (Other test methods with InitializeTest() called at the beginning)
	}
}
