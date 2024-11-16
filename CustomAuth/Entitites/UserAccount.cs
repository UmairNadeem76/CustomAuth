// UserAccounts.cs

// Importing required namespaces
using System.ComponentModel.DataAnnotations; // For defining data annotations
using Microsoft.EntityFrameworkCore; // For database-related configurations, such as indexes

namespace CustomAuth.Entitites // Namespace for entity definitions
{
	// Applying database indexes to ensure unique constraints
	[Index(nameof(Email), IsUnique = true)] // Ensures the Email field is unique in the database
	[Index(nameof(UserName), IsUnique = true)] // Ensures the UserName field is unique in the database
	public class UserAccount
	{
		// Primary Key for the UserAccount table
		[Key]
		public int ID { get; set; }

		// FirstName field with validation attributes
		[Required(ErrorMessage = "First Name Is Required")] // Field is mandatory
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")] // Restricts length to 50 characters
		public string FirstName { get; set; }

		// LastName field with validation attributes
		[Required(ErrorMessage = "Last Name Is Required")] // Field is mandatory
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")] // Restricts length to 50 characters
		public string LastName { get; set; }

		// Email field with validation attributes
		[Required(ErrorMessage = "Email Is Required")] // Field is mandatory
		[MaxLength(100, ErrorMessage = "Max 100 Characters Allowed")] // Restricts length to 100 characters
		public string Email { get; set; }

		// UserName field with validation attributes
		[Required(ErrorMessage = "Username Is Required")] // Field is mandatory
		[MaxLength(20, ErrorMessage = "Max 20 Characters Allowed")] // Restricts length to 20 characters
		public string UserName { get; set; }

		// Password field with validation attributes
		[Required(ErrorMessage = "Password Is Required")] // Field is mandatory
		[MaxLength(20, ErrorMessage = "Max 20 Characters Allowed")] // Restricts length to 20 characters
		public string Password { get; set; }
	}
}
