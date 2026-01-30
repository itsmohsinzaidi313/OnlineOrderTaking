namespace PointofSaleModels.PGDatabaseModels;

public partial class CustomerPhone
{
    public int PhoneId { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetails { get; set; } = [];

    public virtual ICollection<Customer> Customers { get; set; } = [];
}
