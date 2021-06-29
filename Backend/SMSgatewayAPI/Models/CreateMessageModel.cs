using System.Text.Json.Serialization;

namespace SMSgatewayAPI.Models
{
    public class CreateMessageModel
    {
        [JsonPropertyName("deviceId")] public string DeviceId { get; set; }
        [JsonPropertyName("recipient")] public string Recipient { get; set; }
        [JsonPropertyName("content")] public string Content { get; set; }
        [JsonPropertyName("connectionId")] public string ConnectionId { get; set; }
    }
}