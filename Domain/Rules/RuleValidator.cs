using System.Text.Json;
using EthachatShared.Models.WebPush;

namespace Domain.Rules;

public class RuleValidator
{
    public (bool isValid, string message) IsRequestValid(WebPushRequest request)
    {
        if (!request.Subscriptions.Any())
            return (false, "No subscriptions provided");
                
        if (string.IsNullOrWhiteSpace(string.Join(request.PushBodyText, request.PushLink, request.ReceiverUsername)))
            return (false, "No text provided");
        
        return (true, string.Empty);
    }
    
    public (bool isValid, string message) IsConfigValid(string json)
    {
        var fcmConfiguration = JsonSerializer.Deserialize<FcmConfiguration>(json);
        
        if (fcmConfiguration is null)
            return (false, "Invalid FCM Configuration");
        
        return (true, string.Empty);
    }
}