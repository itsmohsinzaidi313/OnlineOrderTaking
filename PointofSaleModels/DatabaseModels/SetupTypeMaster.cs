namespace PointofSaleModels.DatabaseModels;

public partial class SetupTypeMaster
{
    public int TypeId { get; set; }

    public string? TypeName { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<SetupTypeDetail> SetupTypeDetails { get; set; } = new List<SetupTypeDetail>();
}
