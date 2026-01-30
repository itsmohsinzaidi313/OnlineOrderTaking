namespace ImportService.Entities;

public partial class CustomerPhone
{
    public int PhoneId { get; set; }

    public string? PhoneNumber { get; set; }
    
    public bool IsActive { get; set; }
    
    public int? CompanyId { get; set; }

    public int? PhoneTypeId { get; set; }
    public List<Customer> Customers { get; set; } = [];
    public List<CustomerAddressDetail> CustomerAddressDetails { get; set; } = [];
}
