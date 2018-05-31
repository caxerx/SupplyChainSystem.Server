using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class VirtualIdMap
    {

        public int VirtualItemId { get; set; }
        [JsonIgnore] public VirtualItem VirtualItem { get; set; }

        public int ItemId { get; set; }
        [JsonIgnore] public Item Item { get; set; }
    }
}