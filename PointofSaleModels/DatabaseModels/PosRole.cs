using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class PosRole
{
    public int PosRoleId { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<PosRoleActionMapping> PosRoleActionMappings { get; set; } = new List<PosRoleActionMapping>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
