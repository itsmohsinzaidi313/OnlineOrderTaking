namespace PointofSaleModels.DatabaseModels;

public partial class Discount
{
    public int DiscountId { get; set; }

    public double DiscountPercent { get; set; }

    public TimeOnly DiscountTimeStart { get; set; }

    public TimeOnly DiscountTimeEnd { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public string? DiscountName { get; set; }

    public bool IsOpen { get; set; }

    public bool? IsActiveInWeb { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsAreaWise { get; set; }

    public bool IsDayWise { get; set; }

    public int Priority { get; set; }

    public bool IsActiveInPos { get; set; }

    public int? DiscountTypeId { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsActiveInMobile { get; set; }

    public bool? IsActiveInOdms { get; set; }

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

    public string? VocherCode { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<DiscountAreaMapping> DiscountAreaMappings { get; set; } = new List<DiscountAreaMapping>();

    public virtual ICollection<DiscountDayMapping> DiscountDayMappings { get; set; } = new List<DiscountDayMapping>();

    public virtual ICollection<DiscountOrderTypeMapping> DiscountOrderTypeMappings { get; set; } = new List<DiscountOrderTypeMapping>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
