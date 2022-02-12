using System.Text.Json.Serialization;

namespace SMSgatewayAPI.Models;

public record ServiceStatistics
{
    [JsonPropertyName("sentMessages")] public int SentMessages { get; set; }
    [JsonPropertyName("activeDevices")] public int ActiveDevices { get; set; }
    [JsonPropertyName("registeredDevices")] public int RegisteredDevices { get; set; }
}