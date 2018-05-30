using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class RestaurantType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)][Key]
        public int RestaurantTypeId { get; set; }
        public string RestaurantTypeName { get; set; }
    }
}