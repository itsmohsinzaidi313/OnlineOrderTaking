using System.ComponentModel.DataAnnotations;

namespace PointofSaleModels.PGDatabaseModels;

public class OrderNumberSequence
{
    [Key]
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string CurrentOrderNumber { get; set; } = string.Empty;
}