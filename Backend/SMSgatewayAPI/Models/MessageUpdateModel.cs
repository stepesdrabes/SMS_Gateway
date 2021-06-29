using System.Text.Json.Serialization;

namespace SMSgatewayAPI.Models
{
    public class MessageUpdateModel
    {
        [JsonPropertyName("state")] public int State { get; set; }
    }
}