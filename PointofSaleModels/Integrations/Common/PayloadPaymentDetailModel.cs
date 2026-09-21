using System.Text.Json.Serialization;

namespace PointofSaleModels.Integrations.Common
{
    public class PayloadPaymentDetailModel
    {
        [JsonPropertyName("TaxPercentage")]
        public virtual string TaxPercentage { get; set; } = string.Empty;
        [JsonPropertyName("TaxAmount")]
        public string TaxAmount { get; set; } = string.Empty;
        [JsonPropertyName("DiscountPercentage")]
        public string DiscountPercentage { get; set; } = string.Empty;
        [JsonPropertyName("DiscountAmount")]
        public string DiscountAmount { get; set; } = string.Empty;
        [JsonPropertyName("DeliveryCharges")]
        public virtual string DeliveryCharges { get; set; } = string.Empty;
        [JsonPropertyName("VoucherCode")]
        public virtual string VoucherCode { get; set; } = string.Empty;
        [JsonPropertyName("VoucherName")]
        public virtual string VoucherName { get; set; } = string.Empty;
        [JsonPropertyName("GrandTotal")]
        public virtual string GrandTotal { get; set; } = string.Empty;
    }
}
