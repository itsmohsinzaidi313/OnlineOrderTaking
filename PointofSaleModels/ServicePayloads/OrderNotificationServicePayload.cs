using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class OrderNotificationServicePayload : ServicePayload
    {
        public OrderNotificationServicePayload() : base()
        {

        }

        public OrderNotificationServicePayload(OrderNotificationServicePayload payload) : base(payload)
        {
        }

        public OrderNotificationServicePayload(ServicePayload payload) : base(payload)
        {
        }

        public CustomerOrder CustomerOrder { get; set; }

        public List<string> NotificationKeys { get; set; } = [];
    }
}
