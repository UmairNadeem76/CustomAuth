// AdminControllerIntegrationTests.cs
using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using CustomAuth;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;

public class AdminControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
	private readonly HttpClient _client;

	public AdminControllerIntegrationTests(WebApplicationFactory<Startup> factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task GetAllUsers_NonAdminUser_ReturnsForbidden()
	{
		// Arrange
		var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users");

		// Generate a non-admin JWT token
		var token = TokenHelper.GenerateToken("user@example.com", isAdmin: false);

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task GetAllUsers_AdminUser_ReturnsOk()
	{
		// Arrange
		var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users");

		// Generate an admin JWT token
		var token = TokenHelper.GenerateToken("admin@example.com", isAdmin: true);

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}
