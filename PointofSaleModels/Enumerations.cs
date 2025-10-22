namespace PointofSaleModels
{

    public enum UserRole
    {
        Undefined,
        Admin,
        User
    }
    public enum PaymentType
    {
        Undefined,
        Cash,
        Card,
        Credit,
        Voucher,
        FreeofCost
    }

    public enum PaymentStatus
    {
        Undefined,
        Pending,
        Paid,
        Refunded
    }

    public enum OrderStatus
    {
        Undefined,
        Pending,
        Preparing,
        Ready,
        Served,
        InTransit,
        Delivered,
        Cancelled,
        Confirmed,
        Dispatch,
        Paid,
        OnHold
    }

    public enum OrderType
    {
        Undefined,
        DineInIndoor,
        DineInOutdoor,
        TakeAway,
        Delivery
    }

    public enum TableStatus
    {
        Undefined,
        Available,
        Reserved,
        Occupied
    }

    public enum ValueType
    {
        Undefined,
        Amount,
        Percentage
    }
}
