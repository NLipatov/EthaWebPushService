using Application.UseCase;
using Domain;
using IOChannel.Extensions;
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
                var result = await sender.SendPush(request);

                await context.WriteAsync(result);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                await context.WriteAsync(e);
            }
        });

        app.Run();
    }
}