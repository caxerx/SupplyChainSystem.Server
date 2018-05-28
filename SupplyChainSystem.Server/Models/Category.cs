using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChainSystem.Server.Models
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int CategoryId { get; set; }

        [Required]public string CategoryName { get; set; }

        public ICollection<CategoryItem> CategoryItems { get; set; }
    }
}