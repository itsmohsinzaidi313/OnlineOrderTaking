namespace PointofSaleModels.Application
{
    public class Customer
    {
        public int PhoneId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string SelectedAddress { get; set; } = string.Empty;
        public List<string> Addresses { get; set; } = [];
    }
}
