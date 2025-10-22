namespace PointofSaleModels.Application
{
    public class VoucherPayment : PaymentMethod
    {
        public string VoucherNumber { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;

        public VoucherPayment()
        {
            Type = PaymentType.Voucher;
            Amount = 0.0;
        }
    }
}
