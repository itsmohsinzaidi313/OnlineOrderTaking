using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class ClientNotificationServicePayload : ServicePayload
    {
        public ClientNotificationServicePayload() : base()
        {

        }

        public ClientNotificationServicePayload(ClientNotificationServicePayload payload) : base(payload)
        {
        }

        public ClientNotificationServicePayload(ServicePayload payload) : base(payload)
        {
        }

        public CustomerOrder CustomerOrder { get; set; }

        public List<string> NotificationKeys { get; set; } = [];
    }
}
