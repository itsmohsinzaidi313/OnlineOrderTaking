namespace ExportService.Entities
{
    public class BranchDetail
    {
        public int? BranchDetailId { get; set; }

        public int BranchId { get; set; }

        public int AreaId { get; set; }

        public string? AreaName { get; set; }

        public TimeSpan? AreaStartTime { get; set; }

        public TimeSpan? AreaEndTime { get; set; }

        public int? DeliveryTime { get; set; }

        public double? MinimumOrder { get; set; }

        public double? DeliveryCharges { get; set; }

        public bool? IsEnabled { get; set; }

        public double? DeliveryChargesWaiveOffLimit { get; set; }

        
        public bool IsActive { get; set; } = true;
        public BranchDetail CopyWith(BranchDetail instance)
        {
            return new BranchDetail
            {
                BranchDetailId = instance.BranchDetailId,
                BranchId = instance.BranchId,
                AreaId = instance.AreaId,
                AreaName = instance.AreaName,
                AreaStartTime = instance.AreaStartTime,
                AreaEndTime = instance.AreaEndTime,
                DeliveryTime = instance.DeliveryTime,
                MinimumOrder = instance.MinimumOrder,
                DeliveryCharges = instance.DeliveryCharges,
                IsEnabled = instance.IsEnabled,
                DeliveryChargesWaiveOffLimit = instance.DeliveryChargesWaiveOffLimit,
                IsActive = instance.IsActive
            };
        }
    }
}
