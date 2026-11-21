using System;
using System.Threading.Tasks;
using Business_Logic.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank_System.Middleware
{
    // Converts exceptions thrown by the Business layer into proper HTTP responses (ProblemDetails)
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // the client disconnected: nothing to answer
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogError(ex, "Exception after the response had started.");
                    throw;
                }

                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception ex)
        {
            var (status, title, detail) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found", ex.Message),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict", ex.Message),
                BusinessRuleException => (StatusCodes.Status400BadRequest, "Business Rule Violation", ex.Message),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message),
                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Conflict",
                    "The data was changed by another request. Please reload and try again."),
                DbUpdateException => (StatusCodes.Status409Conflict, "Conflict",
                    "The operation conflicts with existing data."),
                _ => (StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Unhandled exception");
            else if (ex is DbUpdateException)
                _logger.LogWarning(ex, "Database update conflict");

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
