namespace PointofSaleModels.DatabaseModels;

public partial class TblFiscalMonth
{
    public int FiscalMonthId { get; set; }

    public int? YearId { get; set; }

    public int? Month { get; set; }

    public bool? IsLock { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public decimal? ClosingBalance { get; set; }

    public int? Year { get; set; }
}
