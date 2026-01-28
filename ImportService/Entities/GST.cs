namespace ImportService.Entities
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

        public GST CopyWith(GST instance)
        {
            return new GST
            {
                GSTId = instance.GSTId,
                GSTPercentage = instance.GSTPercentage,
                CityId = instance.CityId,
                CompanyId = instance.CompanyId,
                IsActive = instance.IsActive,
                GSTName = instance.GSTName,
                PaymentModeId = instance.PaymentModeId
            };
        }
    }
}
