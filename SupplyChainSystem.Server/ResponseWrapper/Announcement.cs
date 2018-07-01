using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.ResponseWrapper
{
    public class Announcement
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime RemovalTime { get; set; }
    }
}