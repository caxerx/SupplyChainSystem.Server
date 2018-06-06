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

    public class ItemRequest
    {
        public string VirtualItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class UserRequest
    {
        public UserType UserType { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }
    }
}