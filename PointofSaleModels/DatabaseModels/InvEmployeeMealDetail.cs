namespace PointofSaleModels.DatabaseModels;

public partial class InvEmployeeMealDetail
{
    public int EmployeeMealDetailId { get; set; }

    public int EmployeeMealMasterId { get; set; }

    public int ProductDetailId { get; set; }

    public double Quantity { get; set; }

    public double ApprovedQuantity { get; set; }

    public int? Level3UnitId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual InvEmployeeMealMaster EmployeeMealMaster { get; set; } = null!;

    public virtual InvSetupUnit? Level3Unit { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
