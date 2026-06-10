using System.Text.Json.Serialization;

namespace PointofSaleModels.Integrations.Common
{
    public class PayloadSubItemModel
    {
        [JsonPropertyName("POSCode")]
        public string POSCode { get; set; } = string.Empty;
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("Category")]
        public string Category { get; set; } = string.Empty;
        [JsonPropertyName("Quantity")]
        public string Quantity { get; set; } = string.Empty;
        [JsonPropertyName("Price")]
        public string Price { get; set; } = string.Empty;
    }
}
