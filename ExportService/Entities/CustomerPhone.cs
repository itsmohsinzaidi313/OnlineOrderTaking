namespace ExportService.Entities;

public partial class CustomerPhone
{
    public int PhoneId { get; set; }

    public string? PhoneNumber { get; set; }
    
    public bool IsActive { get; set; }
    
    public int? CompanyId { get; set; }

    public int? PhoneTypeId { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}
