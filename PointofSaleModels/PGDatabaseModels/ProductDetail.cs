using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class ProductDetail
{
    public int ProductDetailId { get; set; }

    public int ProductId { get; set; }

    public int SizeId { get; set; }

    public string? SizeName { get; set; }

    public double Price { get; set; }

    public double TaxPercent { get; set; }

    public bool IsActive { get; set; }

    public bool OnlyForDeal { get; set; }

    public bool IsEnable { get; set; }

    public int? FlavourId { get; set; }

    public string? FlavourName { get; set; }

    public bool IsTopping { get; set; }

    public bool IsSaleable { get; set; }

    public int? ParentProductDetailId { get; set; }

    public double? FuturePrice { get; set; }

    public double? PreviousPrice { get; set; }

    public bool IsDealDirectPunch { get; set; }

    public bool IsOpen { get; set; }

    public bool IsPromotion { get; set; }

    public string? RemoteId { get; set; }

    public bool IsBestSeller { get; set; }

    public double? PriceBeforeDiscount { get; set; }

    public virtual ICollection<DealDescription> DealDescriptions { get; set; } = new List<DealDescription>();

    public virtual ICollection<DiscountProductDetailMapping> DiscountProductDetailMappings { get; set; } = new List<DiscountProductDetailMapping>();

    public virtual Flavour? Flavour { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductDetailBranchMapping> ProductDetailBranchMappings { get; set; } = new List<ProductDetailBranchMapping>();

    public virtual ICollection<ProductDetailOrderSourcePriceMapping> ProductDetailOrderSourcePriceMappings { get; set; } = new List<ProductDetailOrderSourcePriceMapping>();

    public virtual ProductSize Size { get; set; } = null!;
}
