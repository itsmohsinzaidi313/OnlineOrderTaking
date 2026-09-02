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
        public string Token { get; set; } = string.Empty;
        public string Order { get; set; } = string.Empty;
        public string RemoteId { get; set; } = string.Empty;
        public T OrderPayload { get; set; }
    }
}
