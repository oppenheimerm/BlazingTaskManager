
using Microsoft.AspNetCore.Http;

namespace BlazingTaskManager.Shared.Middleware
{
    /// <summary>
    /// Prevent client access via the API service directly.  All clients must access the 
    /// service via our API Gateway.
    /// </summary>
    public class APIGatewayListener(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //  Extract specific header for the request
            var signedHeader = context.Request.Headers[AppConstants.ApiGateway];

            // If null, request is not coming from APIGatway
            if (signedHeader.FirstOrDefault() is null)
            {
                // Client is accessing service directly(Which we DONT want)
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync(AppConstants.ServiceIsUnavailable503);
                return;
            }
            else
            {
                // Excute next middleware
                await next(context);
            }
        }
    }
}
