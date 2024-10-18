using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CustomAuth.Models
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Username Or Email Is Required")]
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")]
		[DisplayName("Username Or Email")]
		public string UserNameOrEmail { get; set; }

		[Required(ErrorMessage = "Password Is Required")]
		[StringLength(20, MinimumLength = 8, ErrorMessage = "Max 20 & Minimum 8 Characters Are Allowed")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}