using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class BlanketPurchaseAgreementDetails
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }

        //public int PriceBreak

        public string Currency { get; set; }

        public Agreement Agreement { get; set; }
    }
}