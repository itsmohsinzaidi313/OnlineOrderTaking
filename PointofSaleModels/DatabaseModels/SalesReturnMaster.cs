namespace PointofSaleModels.DatabaseModels;

public partial class SalesReturnMaster
{
    public int SalesReturnId { get; set; }

    public string SalesReturnNumber { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public int BranchId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? OrderMasterId { get; set; }

    public double? NetAmount { get; set; }

    public int? CompanyId { get; set; }

    public double DiscountPercent { get; set; }

    public virtual BranchMaster Branch { get; set; } = null!;

    public virtual SetupCompany? Company { get; set; }

    public virtual OrderMaster? OrderMaster { get; set; }

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();
}
