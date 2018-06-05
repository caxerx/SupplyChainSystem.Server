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
        ROOT = 999 //FOR DEBUG
    }
}