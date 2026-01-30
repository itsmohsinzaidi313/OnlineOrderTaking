using System.ComponentModel.DataAnnotations;

namespace PointofSaleModels.PGDatabaseModels;

public class BranchOrderSequence
{
    public long BranchId { get; set; }
    public long LastValue { get; set; }
}