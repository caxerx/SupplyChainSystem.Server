using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class ContractPurchaseAgreementLine
    {
        [ForeignKey("Agreement")] public int AgreementId { get; set; }

        [JsonIgnore] public int ItemId { get; set; }

        public Item Item { get; set; }

        public double Quantity { get; set; }

        [JsonIgnore] public Agreement Agreement { get; set; }
    }
}