namespace PointofSaleModels.PGDatabaseModels;

public partial class Flavour
{
    public int FlavourId { get; set; }

    public string FlavourName { get; set; } = null!;

    public int? CompanyId { get; set; }

    public bool IsActive { get; set; }
}
