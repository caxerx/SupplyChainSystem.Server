﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class BlanketPurchaseAgreementLine
    {
        [ForeignKey("Agreement")] public int AgreementId { get; set; }
        [JsonIgnore] public int ItemId { get; set; }
        [Required] public double PromisedQuantity { get; set; }
        [Required] public double MinimumQuantity { get; set; }
        [Required] public string Unit { get; set; }
        [Required] public double Price { get; set; }

        [Required] public double UsedQuantity { get; set; }

        public Item Item { get; set; }
        [JsonIgnore] public Agreement Agreement { get; set; }
    }
}