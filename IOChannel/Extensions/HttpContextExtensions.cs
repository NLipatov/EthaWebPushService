using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace IOChannel.Extensions;

internal static class HttpContextExtensions
{
    internal static async Task WriteAsync(this HttpContext context, Result result)
    {
        context.Response.StatusCode = result.StatusCode;
        if (!string.IsNullOrWhiteSpace(result.Message))
            await context.Response.WriteAsync(result.Message);
    }
    
    internal static async Task WriteAsync(this HttpContext context, Exception exception)
    {
        context.Response.StatusCode = 500;
        if (!string.IsNullOrWhiteSpace(exception.Message))
            await context.Response.WriteAsync(exception.Message);
    }
}