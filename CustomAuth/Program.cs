// Program.cs

// Importing required namespaces
using Microsoft.AspNetCore.Authentication.JwtBearer; // For configuring JWT authentication
using Microsoft.EntityFrameworkCore; // For database context and interactions
using Microsoft.IdentityModel.Tokens; // For token validation parameters
using Microsoft.AspNetCore.Builder; // For building the web application
using Microsoft.Extensions.DependencyInjection; // For adding services to the dependency injection container
using Microsoft.Extensions.Hosting; // For environment-specific configurations
using System.Text; // For encoding the secret key
using System.Security.Claims; // For handling claims-based authentication
using System.IdentityModel.Tokens.Jwt; // For handling JWT tokens
using CustomAuth.Entitites; // For custom entities like `AppDbContext`
using Serilog; // For logging

// Create a builder for configuring and building the application
var builder = WebApplication.CreateBuilder(args);

// 1. Configure Serilog as the logging provider
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration) // Read configuration from appsettings.json
	.Enrich.FromLogContext() // Add contextual information to logs
	.CreateLogger();

// Replace the default logging with Serilog
builder.Host.UseSerilog();

// Configure database context with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add MVC controllers to the service container
builder.Services.AddControllers();

// Configure JWT-based authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true, // Ensure the token has a valid signing key
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("8DpE3PAHFEhVg5uOCzpqIDrxTy18XD9eJ+++XVyrbXAzYAIEpNltoUjEA3f+5G9X")), // Secret key
		ValidateIssuer = false, // Skip issuer validation
		ValidateAudience = false // Skip audience validation
	};
});

// Configure CORS to allow specific origins, methods, and headers
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		policyBuilder =>
		{
			policyBuilder
				.WithOrigins("http://localhost:3000") // Allow requests from this origin
				.AllowAnyMethod() // Allow all HTTP methods
				.AllowAnyHeader() // Allow all headers
				.AllowCredentials(); // Allow cookies and credentials
		});
});

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy =>
	{
		policy.RequireRole("IsAdmin", "True"); // Require a role claim indicating admin access
	});
});

// Build the application
var app = builder.Build();

// Configure middleware for different environments
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage(); // Detailed error pages for development
}
else
{
	app.UseExceptionHandler("/error"); // Custom error handling in production
	app.UseHsts(); // Enforce HTTP Strict Transport Security in production
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Serve static files
app.UseRouting(); // Enable endpoint routing

// Enable CORS before authentication
app.UseCors("AllowAllOrigins");

// 2. Use Serilog for request logging
app.UseSerilogRequestLogging();

// Custom middleware to extract UserID and role from the JWT
app.Use(async (context, next) =>
{
	var token = context.Request.Cookies["jwt"]; // Retrieve JWT from cookies
	if (!string.IsNullOrEmpty(token))
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes("8DpE3PAHFEhVg5uOCzpqIDrxTy18XD9eJ+++XVyrbXAzYAIEpNltoUjEA3f+5G9X");

		try
		{
			// Validate the token and extract claims
			var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero
			}, out var validatedToken);

			var jwtToken = (JwtSecurityToken)validatedToken;
			var userId = jwtToken.Claims.First(x => x.Type == "UserID").Value;
			var role = jwtToken.Claims.First(x => x.Type == "IsAdmin").Value;

			// Add extracted claims to the user context
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId),
				new Claim(ClaimTypes.Role, role)
			};
			var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
			context.User.AddIdentity(identity);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Token validation failed for token: {Token}", token); // Log token validation failures
		}
	}

	await next.Invoke(); // Pass control to the next middleware
});

// Enable authentication middleware to handle JWT and other authentication schemes
app.UseAuthentication();

// Enable authorization middleware to enforce access control policies
app.UseAuthorization();

// Add custom exception middleware to handle unhandled exceptions globally
app.UseMiddleware<ExceptionMiddleware>();

// Map controllers to their endpoints
app.MapControllers();

// Start the application
try
{
	Log.Information("Starting Web Host"); // Log startup information
	app.Run(); // Run the web application
}
catch (Exception ex)
{
	Log.Fatal(ex, "Host Terminated Unexpectedly"); // Log critical startup errors
}
finally
{
	Log.CloseAndFlush(); // Ensure all logs are written before exiting
}
