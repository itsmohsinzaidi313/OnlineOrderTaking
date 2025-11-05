namespace PointofSaleModels.DatabaseModels;

public partial class TblPayOff
{
    public int PayOffId { get; set; }

    public int VoucherMasterId { get; set; }

    public bool IsActive { get; set; }
}
