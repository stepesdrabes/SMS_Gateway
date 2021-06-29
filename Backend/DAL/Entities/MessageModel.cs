using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DAL.Enums;

namespace DAL.Entities
{
    public record MessageModel
    {
        [Key] [JsonPropertyName("messageId")] public ulong MessageId { get; set; }
        [JsonPropertyName("deviceId")] public string DeviceId { get; set; }
        [JsonPropertyName("recipient")] public string Recipient { get; set; }
        [JsonPropertyName("content")] public string Content { get; set; }
        [JsonPropertyName("state")] public MessageState State { get; set; }
        [JsonPropertyName("sentAt")] public long SentAt { get; set; }

        [JsonIgnore] public bool Updated { get; set; }
        [JsonIgnore] public string ConnectionId { get; set; }
        [JsonPropertyName("proceededAt")] public long? ProceededAt { get; set; }
    }
}