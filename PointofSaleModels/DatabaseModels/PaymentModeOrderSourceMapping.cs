namespace PointofSaleModels.DatabaseModels;

public partial class PaymentModeOrderSourceMapping
{
    public int MappingId { get; set; }

    public int? PaymentModeId { get; set; }

    public int? OrderSourceId { get; set; }

    public int? CompanyId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual SetupMasterDetail? OrderSource { get; set; }

    public virtual SetupMasterDetail? PaymentMode { get; set; }
}
