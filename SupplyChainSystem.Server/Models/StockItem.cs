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
    public class StockItem
    {
        [ForeignKey("Stock")] public int StockId { get; set; }
        [JsonIgnore] public Stock Stock { get; set; }

        public int VirtualItemId { get; set; }
        [JsonIgnore] public VirtualItem VirtualItem { get; set; }

        public int Quantity { get; set; }
    }
}