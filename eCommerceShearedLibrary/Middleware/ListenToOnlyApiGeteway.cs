
using Microsoft.AspNetCore.Http;

namespace eCommerceShearedLibrary.Middleware
{
    public class ListenToOnlyApiGeteway(RequestDelegate next)
    {
         public async Task InvokeAsync(HttpContext context)
        {
            //Extract Specific header from the request
            var signedHeader = context.Request.Headers["api-Geteway"];

            //Null means the request not coming from the Api Geteway 
            if ( signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry servicen is Unavailable");
                return;
            }
            else
            {
                await next(context);
            }

        }
    }
}
