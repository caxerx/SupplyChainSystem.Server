using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChainSystem.Server.Models
{
    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int Id { get; set; }

        [Required] [MaxLength(20)] public string SupplierItemId { get; set; }
        [Required] public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        [Required] public int SupplierId { get; set; }

        public ICollection<VirtualIdMap> VirtualIdMap { get; set; }
    }
}