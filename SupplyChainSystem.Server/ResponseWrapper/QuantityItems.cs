using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.ResponseWrapper
{
    public class QuantityItems
    {
        public string ItemId { get; set; }
        public int PromisedQuantity { get; set; }
        public int MinimumQuantity { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
    }
}