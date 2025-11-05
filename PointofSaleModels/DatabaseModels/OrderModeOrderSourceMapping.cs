namespace PointofSaleModels.DatabaseModels;

public partial class OrderModeOrderSourceMapping
{
    public int MappingId { get; set; }

    public int? OrderModeId { get; set; }

    public int? OrderSourceId { get; set; }

    public int? CompanyId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual SetupMasterDetail? OrderMode { get; set; }

    public virtual SetupMasterDetail? OrderSource { get; set; }
}
