//ErrorViewModel.cs
namespace CustomAuth.Models
{
	// The ErrorViewModel class is used to represent error details in the application
	public class ErrorViewModel
	{
		// Property to hold the unique identifier for the request
		public string? RequestId { get; set; } // Nullable property to store the Request ID, which can be used for tracking errors

		// A computed property that determines if the RequestId should be displayed
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
		// Returns true if RequestId is not null or empty, indicating that it is available for display
	}
}
