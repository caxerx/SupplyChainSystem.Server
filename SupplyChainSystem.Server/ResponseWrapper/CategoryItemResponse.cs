using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server
{
    public class CategoryItemResponse
    {
        public Category Category { get; set; }
        public ICollection<string> VirtualItemId { get; set; }
    }
}
