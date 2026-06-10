using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PointofSaleModels.Integrations.Common
{
    public class PayloadProductModel
    {
        [JsonPropertyName("POSCode")]
        public virtual string POSCode { get; set; } = string.Empty;
        [JsonPropertyName("Name")]
        public virtual string Name { get; set; } = string.Empty;
        [JsonPropertyName("Quantity")]
        public virtual string Quantity { get; set; } = string.Empty;
        [JsonPropertyName("Price")]
        public virtual string Price { get; set; } = string.Empty;
        [JsonPropertyName("DiscountedPrice")]
        public virtual string DiscountedPrice { get; set; } = string.Empty;
        [JsonPropertyName("Comments")]
        public virtual string Comments { get; set; } = string.Empty;
        [JsonPropertyName("DealItem")]
        public List<PayloadSubItemModel> DealItems { get; set; } = new List<PayloadSubItemModel>();
    }
}
