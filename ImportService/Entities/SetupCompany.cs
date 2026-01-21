namespace DataMigration.Domain.Entities
{
    public class SetupCompany
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public string? EmailAddress { get; set; }
        public string? Contact1 { get; set; }
        public string? Contact2 { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ApiUrl { get; set; }
        public int? BusinessTypeId { get; set; }
        public bool? IsEnable { get; set; }
    }

}
