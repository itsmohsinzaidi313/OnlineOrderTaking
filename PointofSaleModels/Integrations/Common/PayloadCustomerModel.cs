using System.Text.Json.Serialization;

namespace PointofSaleModels.Integrations.Common
{
    public class PayloadCustomerModel
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("FirstName")]
        public virtual string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("LastName")]
        public virtual string LastName { get; set; } = string.Empty;
        [JsonPropertyName("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
        [JsonPropertyName("AlternateNumber")]
        public string AlternateNumber { get; set; } = string.Empty;
        [JsonPropertyName("Address")]
        public virtual string Address { get; set; } = string.Empty;

        [JsonPropertyName("Landmark")]
        public virtual string Landmark { get; set; } = string.Empty;

        [JsonPropertyName("Instructions")]
        public virtual string Instructions { get; set; } = string.Empty;
    }
}
