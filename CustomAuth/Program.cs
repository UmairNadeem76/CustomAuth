// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CustomAuth.Entitites;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add services to the container
builder.Services.AddControllers();

// Configure authentication to use JWT Bearer
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("8DpE3PAHFEhVg5uOCzpqIDrxTy18XD9eJ+++XVyrbXAzYAIEpNltoUjEA3f+5G9X")), // Use a secure and stored secret key
		ValidateIssuer = false,
		ValidateAudience = false
	};
});



// Configure CORS to allow specific origins, methods, and headers
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		policyBuilder =>
		{
			policyBuilder
				.WithOrigins("http://localhost:3000") // Allow specified origin
				.AllowAnyMethod() // Allow any HTTP method (GET, POST, etc.)
				.AllowAnyHeader() // Allow any HTTP header
				.AllowCredentials(); // Allow cookies and other credentials
		});
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy =>
	{
		policy.RequireRole("IsAdmin", "True");
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

// Use custom middleware to extract UserID from the JWT and add it to the ClaimsPrincipal
app.Use(async (context, next) =>
{
	var token = context.Request.Cookies["jwt"];
	if (!string.IsNullOrEmpty(token))
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes("8DpE3PAHFEhVg5uOCzpqIDrxTy18XD9eJ+++XVyrbXAzYAIEpNltoUjEA3f+5G9X");

		try
		{
			var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero
			}, out var validatedToken);

			// Extract the UserID claim and add it to the context user claims
			var jwtToken = (JwtSecurityToken)validatedToken;
			var userId = jwtToken.Claims.First(x => x.Type == "UserID").Value;
			var role = jwtToken.Claims.First(x => x.Type == "IsAdmin").Value;
			Console.WriteLine("role is " + role);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId),
				new Claim(ClaimTypes.Role, role)
			};
			var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
			context.User.AddIdentity(identity);
		}
		catch
		{
			// Token validation failed
		}
	}

	await next.Invoke();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
