using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupplyChainSystem.Server.Models
{
    public class Category
    {
        [Key] public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public ICollection<CategoryItem> CategoryItems { get; set; }
    }
}