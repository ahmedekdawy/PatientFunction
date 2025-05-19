using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace Patient.Functions.Middlewares.ExceptionHandlerMiddleware
{
    public class ExceptionHandlerMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exp)
            {
                await ConvertException(context, exp);
            }
        }

        private async Task ConvertException(FunctionContext context, Exception exception)
        {
            var httpRequest = await context.GetHttpRequestDataAsync();
            var httpResponse = httpRequest!.CreateResponse();
            httpResponse.StatusCode = GetStatusCodeFromException(exception);

            string errors = exception.Message;
            // Handle FluentValidation exceptions
            if (exception is FluentValidation.ValidationException validationException)
            {
                errors = string.Join(" ", validationException.Errors.Select(e => e.ErrorMessage));
            }
            await httpResponse.WriteAsJsonAsync(new { message = errors });
            context.GetInvocationResult().Value = httpResponse;

        }
        private HttpStatusCode GetStatusCodeFromException(Exception exception)
        {
            // Map different exception types to status codes
            return exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ApplicationException => HttpStatusCode.UnprocessableEntity,
                FluentValidation.ValidationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}