using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PointofSaleModels.Integrations.Common
{
    public class PayloadModel
    {
        [JsonPropertyName("OrderId")]
        public virtual string OrderId { get; set; } = string.Empty;
        [JsonPropertyName("OSPId")]
        public virtual string OSPId { get; set; } = string.Empty;
        [JsonPropertyName("OrderType")]
        public virtual string OrderType { get; set; } = string.Empty;
        [JsonPropertyName("OrderStatus")]
        public virtual string OrderStatus { get; set; } = string.Empty;
        [JsonPropertyName("PaymentStatus")]
        public virtual string PaymentStatus { get; set; } = string.Empty;
        [JsonPropertyName("HashCode")]
        public virtual string HashCode { get; set; } = string.Empty;
        [JsonPropertyName("PaymentDetail")]
        public virtual PayloadPaymentDetailModel PaymentDetail { get; set; } = new PayloadPaymentDetailModel();
        [JsonPropertyName("Customer")]
        public virtual PayloadCustomerModel Customer { get; set; } = new PayloadCustomerModel();
        [JsonPropertyName("Products")]
        public virtual List<PayloadProductModel> Products { get; set; } = new List<PayloadProductModel>();

    }
}
