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

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetails { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<CustomerPhone> CustomerPhones { get; set; } = new List<CustomerPhone>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Gst> Gsts { get; set; } = new List<Gst>();

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();

    public virtual ICollection<OrderModeCompanyMapping> OrderModeCompanyMappings { get; set; } = new List<OrderModeCompanyMapping>();

    public virtual ICollection<PaymentMode> PaymentModes { get; set; } = new List<PaymentMode>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
