using System;
using System.Collections.Generic;
using System.Text;

namespace PointofSaleModels.ServicePayloads
{
    public class OrderStatusPayload : ServicePayload
    {
        public OrderStatusPayload() : base()
        {
        }

        public OrderStatusPayload(OrderStatusPayload payload) : base(payload)
        {
        }
        public string OrderNumber { get; set; }
        public int? OrderStatusId { get; set; }
        public int? BranchTransferId { get; set; }
        public object? DataPayload { get; set; }
    }
}
