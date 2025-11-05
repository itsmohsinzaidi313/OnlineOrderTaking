using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class SetupCompany
{
    public int CompanyId { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyLogo { get; set; }

    public string? EmailAddress { get; set; }

    public string? Contact1 { get; set; }

    public string? Contact2 { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? ApiUrl { get; set; }

    public int? BusinessTypeId { get; set; }

    public bool? IsEnable { get; set; }

    public virtual ICollection<BranchMaster> BranchMasters { get; set; } = new List<BranchMaster>();

    public virtual ICollection<Gst> Gsts { get; set; } = new List<Gst>();

    public virtual ICollection<OrderModeCompanyMapping> OrderModeCompanyMappings { get; set; } = new List<OrderModeCompanyMapping>();

    public virtual ICollection<PaymentMode> PaymentModes { get; set; } = new List<PaymentMode>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
