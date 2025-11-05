namespace PointofSaleModels.DatabaseModels;

public partial class Table
{
    public int TableId { get; set; }

    public string? TableName { get; set; }

    public int? BranchId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsOpen { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsReserved { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();

    public virtual ICollection<TableMergeDetail> TableMergeDetails { get; set; } = new List<TableMergeDetail>();

    public virtual ICollection<TableMerge> TableMerges { get; set; } = new List<TableMerge>();
}
