namespace PointofSaleModels.DatabaseModels;

public partial class SetupBank
{
    public int BankId { get; set; }

    public string BankName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public int CompanyId { get; set; }
}
