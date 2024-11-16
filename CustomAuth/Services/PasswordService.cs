//PasswordService.cs

// Importing required namespaces
using Microsoft.CodeAnalysis.Scripting;
using BCrypt.Net; // For hashing and verifying passwords using BCrypt

namespace CustomAuth.Services // Declaring the namespace for organizing related classes and components
{
	// The PasswordService class provides functionality to hash passwords and verify them
	public class PasswordService
	{
		// HashPassword method takes a plain-text password and returns its hashed version
		public string HashPassword(string password)
		{
			// Hashing the password using BCrypt to enhance security
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		// VerifyPassword method checks if the provided plain-text password matches the hashed password
		public bool VerifyPassword(string password, string hashedPassword)
		{
			// Verifying the password against the stored hash using BCrypt
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
	}
}
