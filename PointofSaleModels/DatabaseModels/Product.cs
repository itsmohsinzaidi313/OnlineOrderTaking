namespace PointofSaleModels.DatabaseModels;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int? ProductCategoryId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool IsEnable { get; set; }

    public bool DisplayInPos { get; set; }

    public bool DisplayInWeb { get; set; }

    public bool DisplayInOdms { get; set; }

    public bool DisplayInMobile { get; set; }

    public bool IsDeal { get; set; }

    public string? ProductImage { get; set; }

    public bool IsExpiryMandatory { get; set; }

    public double CommisionValue { get; set; }

    public int? CommisionTypeId { get; set; }

    public string? ProductDescription { get; set; }

    public int SortOrder { get; set; }

    public int? ProductTagId { get; set; }

    public virtual SetupMasterDetail? CommisionType { get; set; }

    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();

    public virtual SetupProductTag? ProductTag { get; set; }
}
