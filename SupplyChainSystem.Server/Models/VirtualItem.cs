using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class VirtualItem
    {
        [Key] [MaxLength(20)] public string VirtualItemId { get; set; }
        [Required] public string VirtualItemName { get; set; }
        public string VirtualItemDescription { get; set; }

        public ICollection<VirtualIdMap> VirtualIdMap { get; set; }
    }
}
