using System.ComponentModel.DataAnnotations;

namespace CustomAuth.Models
{
	public class RegistrationViewModel
	{
		[Required(ErrorMessage = "First Name Is Required")]
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last Name Is Required")]
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Email Is Required")]
		[MaxLength(100, ErrorMessage = "Max 100 Characters Allowed")]
		[RegularExpression(@"^[\w\.-]+@[\w\.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please Enter Valid Email")]

		public string Email { get; set; }

		[Required(ErrorMessage = "Username Is Required")]
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Password Is Required")]
		[StringLength(20, MinimumLength = 8, ErrorMessage = "Max 20 & Minimum 8 Characters Are Allowed")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}