using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using GGHN.DigitalLearning.Application.Exceptions;

namespace GGHN.DigitalLearning.Api.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        string type;
        string title;
        int status;
        string detail;
        Dictionary<string, object?>? extensions = null;

        switch (exception)
        {
            case RegistrationValidationException regEx:
                type = regEx.Type;
                title = regEx.Title;
                status = regEx.Status;
                detail = regEx.Message;
                extensions = new Dictionary<string, object?> { ["errors"] = regEx.Errors.ToArray() };
                break;

            case AccountLockoutException lockEx:
                type = lockEx.Type;
                title = lockEx.Title;
                status = lockEx.Status;
                detail = lockEx.Message;
                extensions = new Dictionary<string, object?>();
                if (lockEx.LockoutEnd.HasValue)
                    extensions["lockoutEnd"] = lockEx.LockoutEnd.Value.ToString("o");
                break;

            case EmailNotConfirmedException emailEx:
                type = emailEx.Type;
                title = emailEx.Title;
                status = emailEx.Status;
                detail = emailEx.Message;
                extensions = new Dictionary<string, object?> { ["email"] = emailEx.Email };
                break;

            case InvalidCredentialsException invEx:
                type = invEx.Type;
                title = invEx.Title;
                status = invEx.Status;
                detail = invEx.Message;
                break;

            case KeyNotFoundException:
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                title = "Not found";
                status = StatusCodes.Status404NotFound;
                detail = exception.Message;
                break;

            case UnauthorizedAccessException:
                type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                title = "Unauthorized";
                status = StatusCodes.Status401Unauthorized;
                detail = "Authentication is required to access this resource.";
                break;

            case InvalidOperationException:
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                title = "Bad request";
                status = StatusCodes.Status400BadRequest;
                detail = exception.Message;
                break;

            case ArgumentException:
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                title = "Bad request";
                status = StatusCodes.Status400BadRequest;
                detail = exception.Message;
                break;

            default:
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                title = "Internal server error";
                status = StatusCodes.Status500InternalServerError;
                detail = "An unexpected error occurred. Please try again later.";
                break;
        }

        context.Response.StatusCode = status;

        var problem = new Dictionary<string, object?>
        {
            ["type"] = type,
            ["title"] = title,
            ["status"] = status,
            ["detail"] = detail
        };

        if (extensions is not null)
        {
            foreach (var kvp in extensions)
                problem[kvp.Key] = kvp.Value;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }));
    }
}