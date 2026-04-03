namespace ExportService.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? Title { get; set; }

    public string? CustomerName { get; set; }

    public bool IsActive { get; set; }

    public int? PhoneId { get; set; }

    public bool IsPrimary { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsTaxPayer { get; set; }

    public bool? IsApprovedVendor { get; set; }

    public string? Ntn { get; set; }

    public string? Gst { get; set; }

    public string? Sst { get; set; }

    public string? Cnic { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
}
