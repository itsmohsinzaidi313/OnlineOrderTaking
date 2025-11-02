using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointofSaleModels.Services
{
    public interface IQueueAction
    {
        public string QueueName();
        public Task OnMessage(RabbitMqTransport transport);
    }
}
