using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class PlannedPurchaseAgreementDetails
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }

        public int Period { get; set; }
        public string Unit { get; set; }
        public int PurchaseOrderRevision { get; set; }
    }
}