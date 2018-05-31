using System.Collections.Generic;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server
{
    public class Request
    {
    }

    public class IdRequest
    {
        public string Id { get; set; }
    }

    public class IntIdRequest
    {
        public int Id { get; set; }
    }

    public class NameRequest
    {
        public string Name { get; set; }
    }

    public class RequestRequest
    {
        public ICollection<ItemRequest> Items { get; set; }
    }

    public class ItemRequest
    {
        public string VirtualItemId { get; set; }
        public int Quantity { get; set; }
    }
}