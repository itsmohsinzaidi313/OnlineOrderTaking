using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class CustomerPhone
{
    public int PhoneId { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetails { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
