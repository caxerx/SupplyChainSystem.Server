using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class Restaurant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; }

        public string RestaurantLocation { get; set; }

        public int RestaurantTypeId { get; set; }
        [JsonIgnore] public RestaurantType RestaurantType { get; set; }

        public int StockId { get; set; }
        [JsonIgnore] public Stock Stock { get; set; }

        public ICollection<RestaurantManager> RestaurantManager { get; set; }
    }
}