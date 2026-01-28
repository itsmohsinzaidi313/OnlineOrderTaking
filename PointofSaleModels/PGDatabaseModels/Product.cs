namespace PointofSaleModels.PGDatabaseModels;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? ProductCategoryId { get; set; }

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

    public bool IsDealPackage { get; set; }
}
