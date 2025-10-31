using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointofSaleModels.Services
{
    public interface IQueueExecution
    {
        public Task OnMessage(RabbitMqTransport transport);
    }
}
