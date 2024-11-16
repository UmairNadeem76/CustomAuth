//RegistrationViewModel.cs
using System.ComponentModel.DataAnnotations; // For adding validation attributes

namespace CustomAuth.Models
{
	// The RegistrationViewModel class is used for capturing user input during registration
	public class RegistrationViewModel
	{
		// Property for storing the first name of the user
		[Required(ErrorMessage = "First Name Is Required")] // Ensures this field is mandatory
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")] // Restricts the input length to a maximum of 50 characters
		public string FirstName { get; set; }

		// Property for storing the last name of the user
		[Required(ErrorMessage = "Last Name Is Required")] // Ensures this field is mandatory
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")] // Restricts the input length to a maximum of 50 characters
		public string LastName { get; set; }

		// Property for storing the email address of the user
		[Required(ErrorMessage = "Email Is Required")] // Ensures this field is mandatory
		[MaxLength(100, ErrorMessage = "Max 100 Characters Allowed")] // Restricts the input length to a maximum of 100 characters
		[RegularExpression(@"^[\w\.-]+@[\w\.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please Enter Valid Email")]
		// Ensures the input is a valid email format using a regular expression
		public string Email { get; set; }

		// Property for storing the username of the user
		[Required(ErrorMessage = "Username Is Required")] // Ensures this field is mandatory
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")] // Restricts the input length to a maximum of 50 characters
		public string UserName { get; set; }

		// Property for storing the password of the user
		[Required(ErrorMessage = "Password Is Required")] // Ensures this field is mandatory
		[StringLength(20, MinimumLength = 8, ErrorMessage = "Max 20 & Minimum 8 Characters Are Allowed")]
		// Restricts the input length to a maximum of 20 characters and a minimum of 8 characters
		[DataType(DataType.Password)] // Specifies that this property represents a password field
		public string Password { get; set; }
	}
}
