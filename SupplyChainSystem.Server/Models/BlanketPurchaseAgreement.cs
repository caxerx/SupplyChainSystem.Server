using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class BlanketPurchaseAgreementLine
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }
        [Required] public int Quantity { get; set; }
        [Required] public string Unit { get; set; }
        [Required] public double Price { get; set; }

        public Agreement Agreement { get; set; }
    }
}