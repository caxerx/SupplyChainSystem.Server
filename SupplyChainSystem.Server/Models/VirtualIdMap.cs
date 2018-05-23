using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class VirtualIdMap
    {
        public VirtualIdMap() { }
        public VirtualIdMap(string itemId, string virtualItemId)
        {
            ItemId = itemId;
            VirtualItemId = virtualItemId;
        }

        public string VirtualItemId { get; set; }
        public VirtualItem VirtualItem { get; set; }

        public string ItemId { get; set; }
        public Item Item { get; set; }
    }
}
