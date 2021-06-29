using System.Text.Json.Serialization;
using DAL.Entities;

namespace SMSgatewayAPI.Models
{
    public record SessionToken
    {
        [JsonPropertyName("token")] public string Token { get; set; }
        [JsonPropertyName("device")] public DeviceModel Model { get; set; }
    }
}