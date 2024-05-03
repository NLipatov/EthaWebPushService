using System.Text.Json;
using Domain;
using Domain.Models;
using Domain.Rules;
using EthachatShared.Models.WebPushNotification;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Application.UseCase;

public class FcmPushSender
{
    private const string EnvFcmKeyName = "FCM_KEY_JSON";
    private GoogleCredential _credential;

    public async Task<Response> SendPush(WebPushRequest? request)
    {
        if (request is null)
            return new Response { StatusCode = 400, Message = $"Invalid {nameof(WebPushRequest)}" };
        
        var ruleValidator = new RuleValidator();

        var fcmConfig = GetConfigJsonFromEnv();
        var (isConfigValid, configMessage) = ruleValidator.IsConfigValid(fcmConfig);
        if (!isConfigValid)
            return new Response { StatusCode = 500, Message = configMessage };

        var (isRequestValid, requestMessage) = ruleValidator.IsRequestValid(request);
        if (!isRequestValid)
            return new Response { StatusCode = 400, Message = requestMessage };

        _credential = GoogleCredential.FromJson(fcmConfig);

        try
        {
            var sendPushesWorkload = CreateSendPushesWorkload(request.Subscriptions, request.PushBodyText);
            await Task.WhenAll(sendPushesWorkload);
        }
        catch (Exception e)
        {
            return new Response { Message = e.Message, StatusCode = 500 };
        }

        return new Response { StatusCode = 200 };
    }

    private string GetConfigJsonFromEnv()
    {
        var envConfig = Environment.GetEnvironmentVariable(EnvFcmKeyName) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(envConfig))
            throw new ArgumentException($"Environment variable {EnvFcmKeyName} is not set.");

        var json = JsonSerializer.Deserialize<string>(envConfig) ?? string.Empty;

        return json;
    }

    private Task[] CreateSendPushesWorkload(NotificationSubscriptionDto[] subscriptions, string pushBodyText)
    {
        Task[] workload = new Task[subscriptions.Length];
        for (int i = 0; i < subscriptions.Length; i++)
        {
            int index = i;

            workload[index] = Task.Run(async () =>
            {
                var notificationMessage = new Message()
                {
                    Notification = new Notification()
                    {
                        Title = "η Chat",
                        Body = pushBodyText
                    },
                    Token = subscriptions[index].FirebaseRegistrationToken
                };

                if (FirebaseApp.DefaultInstance == null)
                {
                    var options = new AppOptions
                    {
                        Credential = _credential
                    };

                    FirebaseApp.Create(options);
                }

                var messaging = FirebaseMessaging.DefaultInstance;
                await messaging.SendAsync(notificationMessage);
            });
        }

        return workload;
    }
}