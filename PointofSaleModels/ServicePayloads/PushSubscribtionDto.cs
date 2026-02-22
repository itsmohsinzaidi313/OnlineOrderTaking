namespace PointofSaleModels.ServicePayloads
{
    public class PushSubscriptionDto
    {
        public string ClientId { get; set; } = default!;
        public string Endpoint { get; set; } = default!;
        public string P256DH { get; set; } = default!;
        public string Auth { get; set; } = default!;
    }
}
