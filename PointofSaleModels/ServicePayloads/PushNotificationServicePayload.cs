namespace PointofSaleModels.ServicePayloads
{
    public class PushNotificationServicePayload : ServicePayload
    {
        public string ClientId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
