namespace ImportService.Entities
{
    public class Discount
    {
        public int DiscountId { get; set; }

        public double DiscountPercent { get; set; }

        public TimeSpan DiscountTimeStart { get; set; }

        public TimeSpan DiscountTimeEnd { get; set; }

        public bool IsActive { get; set; }

        public string? DiscountName { get; set; }

        public bool IsOpen { get; set; }

        public bool? IsActiveInWeb { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsAreaWise { get; set; }

        public bool IsDayWise { get; set; }

        public int Priority { get; set; }

        public bool IsActiveInPOS { get; set; }

        public int? DiscountTypeId { get; set; }

        public int? CompanyId { get; set; }

        public bool? IsActiveInMobile { get; set; }

        public bool? IsActiveInODMS { get; set; }

        public bool IsPercentage { get; set; }

        public bool IsAutoDiscount { get; set; }

        public decimal DiscountCapStart { get; set; }

        public decimal DiscountCapEnd { get; set; }

        public bool IsVoucher { get; set; }

        public bool FirstTimeUserOnly { get; set; }

        public bool DeliveryChangesWaiveOff { get; set; }

        public int MaxCount { get; set; }

        public int MaxCountPerUser { get; set; }

        public int MinOrderAmount { get; set; }

        public bool ApplyWithOtherDiscounts { get; set; }

        public string? VocherCodeStart { get; set; }

        public string? VocherCodeEnd { get; set; }
        
        public Discount CopyWith(Discount instance)
        {
            return new Discount
            {
                DiscountId = instance.DiscountId,
                DiscountPercent = instance.DiscountPercent,
                DiscountTimeStart = instance.DiscountTimeStart,
                DiscountTimeEnd = instance.DiscountTimeEnd,
                IsActive = instance.IsActive,
                DiscountName = instance.DiscountName,
                IsOpen = instance.IsOpen,
                IsActiveInWeb = instance.IsActiveInWeb,
                StartDate = instance.StartDate,
                EndDate = instance.EndDate,
                IsAreaWise = instance.IsAreaWise,
                IsDayWise = instance.IsDayWise,
                Priority = instance.Priority,
                IsActiveInPOS = instance.IsActiveInPOS,
                DiscountTypeId = instance.DiscountTypeId,
                CompanyId = instance.CompanyId,
                IsActiveInMobile = instance.IsActiveInMobile,
                IsActiveInODMS = instance.IsActiveInODMS,
                IsPercentage = instance.IsPercentage,
                IsAutoDiscount = instance.IsAutoDiscount,
                DiscountCapStart = instance.DiscountCapStart,
                DiscountCapEnd = instance.DiscountCapEnd,
                IsVoucher = instance.IsVoucher,
                FirstTimeUserOnly = instance.FirstTimeUserOnly,
                DeliveryChangesWaiveOff = instance.DeliveryChangesWaiveOff,
                MaxCount = instance.MaxCount,
                MaxCountPerUser = instance.MaxCountPerUser,
                MinOrderAmount = instance.MinOrderAmount,
                ApplyWithOtherDiscounts = instance.ApplyWithOtherDiscounts,
                VocherCodeStart = instance.VocherCodeStart,
                VocherCodeEnd = instance.VocherCodeEnd
            };
        }
    }
}
