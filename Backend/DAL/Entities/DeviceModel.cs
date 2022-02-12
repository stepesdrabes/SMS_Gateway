using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DAL.Entities;

public record DeviceModel
{
    [Key] public string DeviceId { get; set; }
    public string Vendor { get; set; }
    public string Model { get; set; }
    public string OsVersion { get; set; }
    [JsonIgnore] public DateTime RegisteredAt { get; set; }
}