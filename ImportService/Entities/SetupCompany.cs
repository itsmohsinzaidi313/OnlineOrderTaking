namespace ImportService.Entities
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

        public SetupCompany CopyWith(SetupCompany instance)
        {
            return new SetupCompany
            {
                CompanyId = instance.CompanyId,
                CompanyName = instance.CompanyName,
                CompanyLogo = instance.CompanyLogo,
                EmailAddress = instance.EmailAddress,
                Contact1 = instance.Contact1,
                Contact2 = instance.Contact2,
                WebsiteUrl = instance.WebsiteUrl,
                ApiUrl = instance.ApiUrl,
                BusinessTypeId = instance.BusinessTypeId,
                IsEnable = instance.IsEnable
            };
        }
    }
}
