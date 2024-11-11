//UserAccounts.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CustomAuth.Entitites
{

	[Index(nameof(Email), IsUnique = true)]
	[Index(nameof(UserName), IsUnique = true)]

	public class UserAccount
	{
		[Key]
		public int ID { get; set; }

		[Required (ErrorMessage = "First Name Is Required")]
		[MaxLength (50, ErrorMessage = "Max 50 Characters Allowed")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last Name Is Required")]
		[MaxLength(50, ErrorMessage = "Max 50 Characters Allowed")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Email Is Required")]
		[MaxLength(100, ErrorMessage = "Max 100 Characters Allowed")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Username Is Required")]
		[MaxLength(20, ErrorMessage = "Max 20 Characters Allowed")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Password Is Required")]
		[MaxLength(20, ErrorMessage = "Max 20 Characters Allowed")]
		public string Password { get; set; }
	}
}