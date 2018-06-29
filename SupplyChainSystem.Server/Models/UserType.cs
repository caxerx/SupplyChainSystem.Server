using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public enum UserType
    {
        Administrator,
        RestaurantManager,
        CategoryManager,
        PurchaseManager,
        WarehouseClerk,
        ROOT = 999 //FOR DEBUG
    }
}