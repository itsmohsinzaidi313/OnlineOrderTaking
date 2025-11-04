namespace PointofSaleModels.DatabaseModels;

public partial class VwGrn
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public int ProductId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? Qty { get; set; }

    public string? BatchNumber { get; set; }

    public string? Barcode { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool Isexpirymandatory { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? BatchId { get; set; }

    public int? BranchId { get; set; }
}
