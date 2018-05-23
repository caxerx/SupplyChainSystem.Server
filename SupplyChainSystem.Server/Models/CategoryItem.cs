using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class CategoryItem
    {
        public string VirtualItemId { get; set; }
        public VirtualItem VirtualItem { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
