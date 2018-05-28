using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChainSystem.Server.Models
{
    public class CategoryItem
    {
        public int VirtualItemId { get; set; }
        public VirtualItem VirtualItem { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}