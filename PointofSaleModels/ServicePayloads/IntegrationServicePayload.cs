using System;
using System.Collections.Generic;
using System.Text;

namespace PointofSaleModels.ServicePayloads
{
    public class IntegrationServicePayload<T> : ServicePayload
    {
        public IntegrationServicePayload() : base()
        {
        }
        public IntegrationServicePayload(ServicePayload servicePayload) : base(servicePayload)
        {
        }
        public T OrderPayload { get; set; }
    }
}
