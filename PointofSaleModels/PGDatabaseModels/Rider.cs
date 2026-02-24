namespace PointofSaleModels.PGDatabaseModels
{
    public class Rider
    {
        public int RiderId { get; set; }
        public string? RiderName { get; set; }
        public string? Contact1 { get; set; }
        public string? Contact2 { get; set; }
        public int? BranchId { get; set; }
        public bool IsActive { get; set; }
    }
}
