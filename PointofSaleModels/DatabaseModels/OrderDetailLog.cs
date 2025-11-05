namespace PointofSaleModels.DatabaseModels;

public partial class OrderDetailLog
{
    public int OrderDetailLogId { get; set; }

    public int? OrderMasterId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? Quantity { get; set; }

    public double? PriceWithoutGst { get; set; }

    public double? AmountWithoutGst { get; set; }

    public int? TypeId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsKot { get; set; }

    public bool IsScreenKot { get; set; }

    public bool IsKotServicePrint { get; set; }

    public string? SpecialInstruction { get; set; }

    public bool ItemFoc { get; set; }

    public virtual OrderMaster? OrderMaster { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual SetupMasterDetail? Type { get; set; }
}
