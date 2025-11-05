namespace PointofSaleModels.DatabaseModels;

public partial class BranchOrderSourceMapping
{
    public int MappingId { get; set; }

    public int? BranchId { get; set; }

    public int? OrderSourceId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual SetupMasterDetail? OrderSource { get; set; }
}
