//LoginViewModel.cs
using System.ComponentModel; // For adding display-related metadata
using System.ComponentModel.DataAnnotations; // For adding validation attributes

namespace CustomAuth.Models
{
	// The LoginViewModel class is used for capturing user input during login
	public class LoginViewModel
	{
		// Property for storing the username or email input
		[Required(ErrorMessage = "Username Or Email Is Required")] // Ensures this field is mandatory
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")] // Restricts the input length to a maximum of 50 characters
		[DisplayName("Username Or Email")] // Sets the display name for UI purposes
		public string UserNameOrEmail { get; set; }

		// Property for storing the password input
		[Required(ErrorMessage = "Password Is Required")] // Ensures this field is mandatory
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Max 100 & Minimum 8 Characters Are Allowed")]
		// Restricts the input length to a maximum of 20 characters and a minimum of 8 characters
		[DataType(DataType.Password)] // Specifies that this property represents a password field
		public string Password { get; set; }
	}
}
