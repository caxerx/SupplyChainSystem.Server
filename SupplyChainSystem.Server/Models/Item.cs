using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupplyChainSystem.Server.Models
{
    public class Item
    {
        [Key] [MaxLength(20)] public string ItemId { get; set; }
        [Required] public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        [Required] public int SupplierId { get; set; }

        public ICollection<VirtualIdMap> VirtualIdMap { get; set; }
    }
}