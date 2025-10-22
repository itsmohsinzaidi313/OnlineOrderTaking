using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class SetupMaster
{
    public int SetupMasterId { get; set; }

    public string? SetupMasterName { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<SetupMasterDetail> SetupMasterDetails { get; set; } = new List<SetupMasterDetail>();
}
