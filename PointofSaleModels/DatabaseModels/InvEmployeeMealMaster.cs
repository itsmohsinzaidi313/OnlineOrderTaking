using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvEmployeeMealMaster
{
    public int EmployeeMealMasterId { get; set; }

    public string EmployeeMealNumber { get; set; } = null!;

    public DateOnly? Date { get; set; }

    public int? BranchId { get; set; }

    public bool IsActive { get; set; }

    public bool IsSubmit { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? UserId { get; set; }

    public string? UserIp { get; set; }

    public int CompanyId { get; set; }

    public bool IsApproved { get; set; }

    public string? EmployeeName { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual SetupCompany Company { get; set; } = null!;

    public virtual ICollection<InvConsumptionMaster> InvConsumptionMasters { get; set; } = new List<InvConsumptionMaster>();

    public virtual ICollection<InvEmployeeMealDetail> InvEmployeeMealDetails { get; set; } = new List<InvEmployeeMealDetail>();
}
