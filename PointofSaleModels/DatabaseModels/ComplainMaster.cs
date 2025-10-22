using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ComplainMaster
{
    public int ComplainMasterId { get; set; }

    public string? ComplainNumber { get; set; }

    public int? OrderMasterId { get; set; }

    public int? ComplainStatusId { get; set; }

    public int? ComplainTypeId { get; set; }

    public int? ComplainCategoryId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? CreatedDateInt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int CompanyId { get; set; }

    public virtual SetupCompany Company { get; set; } = null!;

    public virtual ComplainCategory? ComplainCategory { get; set; }

    public virtual ICollection<ComplainDetail> ComplainDetails { get; set; } = new List<ComplainDetail>();

    public virtual ComplainStatus? ComplainStatus { get; set; }

    public virtual SetupMasterDetail? ComplainType { get; set; }

    public virtual OrderMaster? OrderMaster { get; set; }
}
