using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class SetupCompanySetting
{
    public int SettingId { get; set; }

    public int? SetupDetailId { get; set; }

    public string? SettingValue { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public int? BranchId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual SetupMasterDetail? SetupDetail { get; set; }
}
