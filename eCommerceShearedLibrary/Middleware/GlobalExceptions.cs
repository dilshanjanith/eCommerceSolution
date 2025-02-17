
using eCommerceShearedLibrary.LogEx;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace eCommerceShearedLibrary.Middleware
{
    public class GlobalExceptions(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare default Variables
            string message = "Sorry Internal Server Error Occurred! Kindly Try Again!";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                //Check If Response is too many requests //status code 429
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning!";
                    message = "Too many request made.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                //If Response is UnAuthorized //Status code 401
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not Authorized to access.";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // If Response is Frobidden // status code 403
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are Not Allowd to Access";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                //Log Original Exception to the Console, File, Debugger
                LogExceptions.LogException(ex);

                //Check If Exception is timeout //408 Request TimeOut
                if( ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "time Out";
                    message = "Request time Out... try again";
                    statusCode = (int)StatusCodes.Status408RequestTimeout;
                }

                //If Exception is caught
                //If no more the exceptions then do the defaut 
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statuscode)
        {
            //Display scary-free message to the client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statuscode,
                Title = title

            }), CancellationToken.None);
            return;
        }
    }
}
