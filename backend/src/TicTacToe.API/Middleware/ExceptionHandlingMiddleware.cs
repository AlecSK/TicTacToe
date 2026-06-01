using System.Text.Json;
using TicTacToe.Application.Exceptions;

namespace TicTacToe.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (BusinessException ex)
        {
            await WriteResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteResponse(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(body);
    }
}
