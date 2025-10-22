namespace PointofSaleModels.Application
{
    public class OrderPaymentRequest
    {
        public int Id { get; set; }
        public Tax Tax { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
    }
}
