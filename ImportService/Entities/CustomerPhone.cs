using PointofSaleModels.DatabaseModels;

namespace ImportService.Entities;

public partial class CustomerPhone
{
    public int PhoneId { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedDateInt { get; set; }

    public int? ModifiedDateInt { get; set; }

    public int? CompanyId { get; set; }

    public int? PhoneTypeId { get; set; }
}
