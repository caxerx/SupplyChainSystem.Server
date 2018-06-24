using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class Warehouse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public string WarehouseLocation { get; set; }

        public int StockId { get; set; }
        [JsonIgnore] public Stock Stock { get; set; }
    }
}