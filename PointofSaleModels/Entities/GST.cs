namespace PointofSaleModels.Entities
{
    public class GST
    {
        public int GSTId { get; set; }

        public double? GSTPercentage { get; set; }

        public int? CityId { get; set; }

        public int? CompanyId { get; set; }

        public bool? IsActive { get; set; }

        public string? GSTName { get; set; }

        public int? PaymentModeId { get; set; }
        
    }
}
