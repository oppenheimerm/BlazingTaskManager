using BlazingTaskManager.Shared.APIServiceLogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BlazingTaskManager.Shared.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //  Default exception variables
            string message = "Sorry, internal serve error occured. Please try again.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);
                // if exception == to many request(429)
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Waring";
                    message = "Too many request made.";
                    statusCode = StatusCodes.Status429TooManyRequests;

                    // Modifiy te response header
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if exception == Unauthrized(401)
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Security Warning";
                    message = "Unauthorized access.";
                    statusCode = StatusCodes.Status401Unauthorized;

                    // Modifiy te response header
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if exception == Forbidden(403)
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Security Warning";
                    message = "Access denied.";
                    statusCode = StatusCodes.Status403Forbidden;

                    // Modifiy te response header
                    await ModifyHeader(context, title, message, statusCode);
                }

            }
            catch (Exception ex)
            {

                //  Log original exception to file, debugger, console
                LogException.LogExceptions(ex);

                // Check if timeout exception(408)
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Timeout exception.";
                    message = "Reques timed out. Please try again.";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                // Modifiy te response header
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //  Display friendly message
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new ProblemDetails()
                {
                    Detail = message,
                    Status = statusCode,
                    Title = title
                }), CancellationToken.None);
            return;

        }
    }
}
