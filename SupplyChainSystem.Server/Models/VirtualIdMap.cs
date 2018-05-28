using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChainSystem.Server.Models
{
    public class VirtualIdMap
    {

        public int VirtualItemId { get; set; }
        public VirtualItem VirtualItem { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }
    }
}