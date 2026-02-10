using PointofSaleModels.Application;
using System;
using System.Collections.Generic;
using System.Text;

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
