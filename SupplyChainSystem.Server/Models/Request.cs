using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        public int RestaurantId { get; set; }
        [JsonIgnore] public Restaurant Restaurant { get; set; }

        [ForeignKey("User")] public int Creator { get; set; }
        [JsonIgnore] public User User { get; set; }

        public ICollection<RequestItem> RequestItem { get; set; }
    }
}