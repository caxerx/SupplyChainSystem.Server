using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class ContractPurchaseAgreementLine
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }

        public int ItemId { get; set; }

        public Item Item { get; set; }

        public Agreement Agreement { get; set; }
    }
}