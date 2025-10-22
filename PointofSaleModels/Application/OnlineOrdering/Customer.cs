namespace PointofSaleModels.Application.OnlineOrdering
{
    public class Customer
    {
        public int PhoneId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public Dictionary<string, string> Addresses { get; set; } = [];
        public string Token { get; set; }
    }
}
