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
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("6567e69b55419b2c16b904595c5af62257603b577d1da3b0b6c60375ebbc7bf7bca9ab475b7c6782b7850e3e0311ed32cb0e4d0be9a2f2797ba5d7511e577293da09aa1a3d1cd261bf500d4efd65166cdbb9a752ffabe1c47b970e6336dec0ea24afd3e71bde5e2d0b9fddba56c09870f23ac67926b5e3f15b7b7b856fb3d751ffc7dca579d62c77f530006db3144400a99cf0f53c959a78eb6a2e5a391fca712b61708897bcd072e21524d577105fe3a1bffd658da6ec1cc13a32139713ed0322beb1ef38dbf03f86116ad600bf30e59785ed79f447307a76ab29bd9e504b71be8dead2d1466173e76d362e0c2d8b6c9b34b93040b6de39408469faabee9673")), // Use a secure and stored secret key
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
		var key = Encoding.ASCII.GetBytes("6567e69b55419b2c16b904595c5af62257603b577d1da3b0b6c60375ebbc7bf7bca9ab475b7c6782b7850e3e0311ed32cb0e4d0be9a2f2797ba5d7511e577293da09aa1a3d1cd261bf500d4efd65166cdbb9a752ffabe1c47b970e6336dec0ea24afd3e71bde5e2d0b9fddba56c09870f23ac67926b5e3f15b7b7b856fb3d751ffc7dca579d62c77f530006db3144400a99cf0f53c959a78eb6a2e5a391fca712b61708897bcd072e21524d577105fe3a1bffd658da6ec1cc13a32139713ed0322beb1ef38dbf03f86116ad600bf30e59785ed79f447307a76ab29bd9e504b71be8dead2d1466173e76d362e0c2d8b6c9b34b93040b6de39408469faabee9673");

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

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId)
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
