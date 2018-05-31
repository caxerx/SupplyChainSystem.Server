using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class RestaurantManager
    {
        [Key] public int UserId { get; set; }
        public int RestaurantId { get; set; }

        [JsonIgnore] public Restaurant Restaurant { get; set; }
        [JsonIgnore] public User User { get; set; }
    }
}