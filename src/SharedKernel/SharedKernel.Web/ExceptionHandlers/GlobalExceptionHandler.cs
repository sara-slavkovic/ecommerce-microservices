using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Web.ExceptionHandlers
{
    public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, title, detail) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found", exception.Message),
                BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred.")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
            else
                logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);

            httpContext.Response.StatusCode = statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title = title,
                    Detail = detail,
                    Status = statusCode,
                    Instance = httpContext.Request.Path
                }
            });
        }
    }
}
