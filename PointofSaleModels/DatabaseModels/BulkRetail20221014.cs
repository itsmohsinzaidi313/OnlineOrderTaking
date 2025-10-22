using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class BulkRetail20221014
{
    public string? Department { get; set; }

    public string? Category { get; set; }

    public string? CategorySortOrder { get; set; }

    public string? Product { get; set; }

    public string? IsDealProduct { get; set; }

    public string? Size { get; set; }

    public string? Variant { get; set; }

    public string? Price { get; set; }

    public string? OnlyForDeal { get; set; }

    public string? IsTopping { get; set; }

    public string? IsSaleable { get; set; }

    public string? IsProduction { get; set; }

    public string? PurchaseUnit { get; set; }

    public string? IssuanceUnit { get; set; }

    public string? ConsumeUnit { get; set; }

    public string? PurchaseIssueConversion { get; set; }

    public string? IssueConsumeConversion { get; set; }

    public string? Sku { get; set; }

    public string? ReOrderQuantityInConsume { get; set; }

    public string? Barcode { get; set; }
}
