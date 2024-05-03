using EthachatShared.Models.WebPushNotification;

namespace Domain;

public record WebPushRequest
{
    public string PushBodyText { get; set; } = string.Empty;
    public string PushLink { get; set; } = string.Empty;
    public string ReceiverUsername { get; set; } = string.Empty;
    public NotificationSubscriptionDto[] Subscriptions { get; set; } = [];
}