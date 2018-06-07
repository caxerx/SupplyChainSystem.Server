using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SupplyChainSystem.Server.Models
{
    public enum RequestStatus
    {
        WaitingForProcess,
        Processing,
        Ordered,
        Delivering,
        Delivered
    }
}