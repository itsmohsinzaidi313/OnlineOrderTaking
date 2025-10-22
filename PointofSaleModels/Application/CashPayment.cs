namespace PointofSaleModels.Application
{
    public class CashPayment : PaymentMethod
    {
        public CashPayment()
        {
            Type = PaymentType.Cash;
        }
    }
}
