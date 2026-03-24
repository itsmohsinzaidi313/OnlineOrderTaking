namespace PointofSaleModels.Entities
{
    public class ProductDetailAvailability
    {
        public int ProductDetailAvailableId { get; set; }

        public int? ProductBranchId { get; set; }

        public int? DayId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool? IsActive { get; set; }
        
    }
}
