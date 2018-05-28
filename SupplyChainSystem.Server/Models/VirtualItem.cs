using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChainSystem.Server.Models
{
    public class VirtualItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required] [MaxLength(20)] public string VirtualItemId { get; set; }

        [Required] public string VirtualItemName { get; set; }
        public string VirtualItemDescription { get; set; }

        public ICollection<VirtualIdMap> VirtualIdMap { get; set; }
        public ICollection<CategoryItem> CategoryItem { get; set; }
    }
}