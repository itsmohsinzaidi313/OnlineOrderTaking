namespace PointofSaleModels.DatabaseModels;

public partial class PosAction
{
    public int PosActionId { get; set; }

    public string PosAction1 { get; set; } = null!;

    public virtual ICollection<PosRoleActionMapping> PosRoleActionMappings { get; set; } = new List<PosRoleActionMapping>();
}
