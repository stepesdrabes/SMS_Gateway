using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DAL.Entities
{
    public record DeviceModel
    {
        [Key] [JsonPropertyName("deviceId")] public string DeviceId { get; set; }
        [JsonPropertyName("vendor")] public string Vendor { get; set; }
        [JsonPropertyName("model")] public string Model { get; set; }
        [JsonPropertyName("osVersion")] public string OsVersion { get; set; }
        [JsonIgnore] public DateTime RegisteredAt { get; set; }
    }
}