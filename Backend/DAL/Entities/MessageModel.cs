using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DAL.Enums;

namespace DAL.Entities;

public record MessageModel
{
    [Key] public ulong MessageId { get; set; }
    public string DeviceId { get; set; }
    public string Recipient { get; set; }
    public string Content { get; set; }
    public MessageState State { get; set; }
    public long SentAt { get; set; }

    [JsonIgnore] public bool Updated { get; set; }
    [JsonIgnore] public string ConnectionId { get; set; }
    public long? ProceededAt { get; set; }
}