namespace PointofSaleModels.PGDatabaseModels
{
    public class OrderStatus
    {
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}
