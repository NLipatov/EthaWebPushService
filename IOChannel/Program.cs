using Application.UseCase;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace IOChannel;

public static class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder([]);
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        app.MapPost("/send-web-push", async context =>
        {
            try
            {
                var request = await context.Request.ReadFromJsonAsync<WebPushRequest>();

                var sender = new FcmPushSender();
                var response = await sender.SendPush(request);
                
                context.Response.StatusCode = response.StatusCode;
                if (!string.IsNullOrWhiteSpace(response.Message))
                    await context.Response.WriteAsync(response.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
            }
        });

        app.Run();
    }
}