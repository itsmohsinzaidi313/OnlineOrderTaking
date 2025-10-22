namespace PointofSaleModels.Application
{
    public class CardPayment : PaymentMethod
    {
        public string CardNumber { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
    }
}
