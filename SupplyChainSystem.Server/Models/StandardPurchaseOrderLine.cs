using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class StandardPurchaseOrderLine
    {
        [Key]
        [ForeignKey("StandardPurchaseOrder")]
        public int OrderId { get; set; }

        [ForeignKey("Item")] public int ItemId { get; set; }
        public int Quantity { get; set; }

        public double Price { get; set; }


        public StandardPurchaseOrder StandardPurchaseOrder { get; set; }
        public Item Item { get; set; }
    }
}