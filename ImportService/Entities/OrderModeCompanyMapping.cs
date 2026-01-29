namespace ImportService.Entities
{
    public class OrderModeCompanyMapping
    {
        public int OrderModeMappingId { get; set; }

        public int? OrderModeId { get; set; }

        public int? CompanyId { get; set; }

        public bool IsActive { get; set; }
        public OrderModeCompanyMapping CopyWith(OrderModeCompanyMapping instance)
        {
            return new OrderModeCompanyMapping
            {
                OrderModeMappingId = instance.OrderModeMappingId,
                OrderModeId = instance.OrderModeId,
                CompanyId = instance.CompanyId,
                IsActive = instance.IsActive
            };
        }
    }
}
