namespace PointofSaleModels.PGDatabaseModels;

public partial class SetupMaster
{
    public int SetupMasterId { get; set; }

    public string SetupMasterName { get; set; } = null!;

    public bool IsActive { get; set; }
}
