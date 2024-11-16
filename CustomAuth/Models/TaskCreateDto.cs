// TaskCreateDto.cs

// Importing required namespaces
using Microsoft.Build.Framework; // For validation attributes (Required in this case)

namespace CustomAuth.Models
{
	// The TaskCreateDto class is a Data Transfer Object (DTO) for creating a new task
	public class TaskCreateDto
	{
		// Property for storing the name of the task
		[Required] // Ensures that the Task_Name field must be provided
		public string Task_Name { get; set; }

		// Property for storing the description of the task
		[Required] // Ensures that the Task_Description field must be provided
		public string Task_Description { get; set; }

		// Property for storing the status of the task (e.g., Pending, Completed)
		[Required] // Ensures that the Task_Status field must be provided
		public string Task_Status { get; set; }

		// Property for storing the priority level of the task
		[Required] // Ensures that the Task_Priority field must be provided
		public int Task_Priority { get; set; }
	}
}
