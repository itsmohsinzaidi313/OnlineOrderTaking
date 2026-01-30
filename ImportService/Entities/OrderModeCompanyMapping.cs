namespace ImportService.Entities
{
    public class OrderModeCompanyMapping
    {
        public int OrderModeMappingId { get; set; }

        public int? OrderModeId { get; set; }

        public int? CompanyId { get; set; }

        public bool IsActive { get; set; }
        
    }
}
