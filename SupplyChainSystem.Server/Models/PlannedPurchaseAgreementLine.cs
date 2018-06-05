using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class PlannedPurchaseAgreementLine
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }
        [Required] public int ItemId { get; set; }
        [Required] public double Quantity { get; set; }
        [Required] public string Unit { get; set; }
        [Required] public double Price { get; set; }

        [JsonIgnore] public Item Item { get; set; }
        [JsonIgnore] public Agreement Agreement { get; set; }
    }
}