using System.Collections.Generic;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server
{
    public class CategoryItemResponse
    {
        public Category Category { get; set; }
        public ICollection<string> VirtualItemId { get; set; }
    }
}