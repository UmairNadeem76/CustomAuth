// Program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CustomAuth.Entitites;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add services to the container
builder.Services.AddControllers();

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/api/account/login"; // Redirect path for unauthorized users
	});

// Configure CORS to allow all origins, methods, and headers
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		policyBuilder =>
		{
			policyBuilder
				.WithOrigins("http://localhost:3000")// Allow all origins
				.AllowAnyMethod() // Allow any HTTP method (GET, POST, etc.)
				.AllowAnyHeader() // Allow any HTTP header
				.AllowCredentials(); // Allow cookies and other credentials
		});
});

// Add logging configuration for better error tracking
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	// Show detailed error pages in development
	app.UseDeveloperExceptionPage();
}
else
{
	// In production, use the default error handler and HSTS
	app.UseExceptionHandler("/error");
	app.UseHsts();
}

// Optionally, you can disable HTTPS redirection for debugging purposes
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Enable CORS before authentication and authorization
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
