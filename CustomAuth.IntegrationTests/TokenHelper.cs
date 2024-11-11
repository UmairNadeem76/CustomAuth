// TokenHelper.cs
using System.Security.Claims;
using System.Text;

public static class TokenHelper
{
	public static string GenerateToken(string email, bool isAdmin)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes("YourSecureKey"); // Use your actual key

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, email),
			new Claim("IsAdmin", isAdmin.ToString())
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddHours(1),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}