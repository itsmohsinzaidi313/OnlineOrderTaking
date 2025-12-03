namespace PointofSaleModels.ServicePayloads
{
    public class GetMenuServicePayload : ServicePayload
    {
        public GetMenuServicePayload() : base() { }
        public GetMenuServicePayload(ServicePayload payload) : base(payload) { }
        public object? Menu { get; set; }
    }
}
