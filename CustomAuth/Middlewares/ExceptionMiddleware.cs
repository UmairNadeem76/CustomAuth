// ExceptionMiddleware.cs

// This class provides middleware for handling unhandled exceptions in the application
public class ExceptionMiddleware
{
    // Holds the next middleware in the pipeline
    private readonly RequestDelegate _next;

    // Constructor to initialize the middleware
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next; // Assign the next middleware in the pipeline
    }

    // This method is invoked for each HTTP request passing through the middleware
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request to the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Catch unhandled exceptions and process them
            await HandleExceptionAsync(context, ex);
        }
    }

    // Handles the exception by returning a standardized JSON error response
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Set the response content type to JSON
        context.Response.ContentType = "application/json";

        // Set the HTTP status code to 500 (Internal Server Error)
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Create a response object containing an error message and exception details
        var response = new
        {
            Message = "An Unexpected Error Occurred.", // General error message
            Details = exception.Message // Exception details (for debugging purposes)
        };

        // Serialize the response object to JSON and write it to the HTTP response
        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}
