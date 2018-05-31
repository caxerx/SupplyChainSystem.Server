using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class CategoryItem
    {
        public int VirtualItemId { get; set; }
        [JsonIgnore] public VirtualItem VirtualItem { get; set; }

        public int CategoryId { get; set; }
        [JsonIgnore] public Category Category { get; set; }
    }
}