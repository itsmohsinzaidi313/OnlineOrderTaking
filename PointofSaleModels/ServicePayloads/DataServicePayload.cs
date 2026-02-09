using System;
using System.Collections.Generic;
using System.Text;

namespace PointofSaleModels.ServicePayloads
{
    public class DataServicePayload : ServicePayload
    {
        public DataServicePayload() : base()
        {
        }

        public DataServicePayload(DataServicePayload payload) : base(payload)
        {
            RequestType = payload.RequestType;
        }

        public string RequestType { get; set; }
    }
}
