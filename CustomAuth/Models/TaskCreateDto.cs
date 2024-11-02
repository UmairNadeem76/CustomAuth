using Microsoft.Build.Framework;

namespace CustomAuth.Models
{
	public class TaskCreateDto
	{
		[Required]
		public string Task_Name { get; set; }

		[Required]
		public string Task_Description { get; set; }

		[Required]
		public string Task_Status { get; set; }

		[Required]
		public int Task_Priority { get; set; }
	}
}