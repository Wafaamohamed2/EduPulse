using System.Text.Json;

namespace EduPulse.Models
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                var response = new ApiResponse<object>(false, null, $"Internal server error: {ex.Message}", 500);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
