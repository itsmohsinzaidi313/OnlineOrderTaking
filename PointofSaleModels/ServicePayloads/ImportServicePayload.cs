using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointofSaleModels.ServicePayloads
{
    public class ImportServicePayload : ServicePayload
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
