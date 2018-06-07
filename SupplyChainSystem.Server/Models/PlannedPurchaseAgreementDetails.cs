using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class PlannedPurchaseAgreementDetails
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }
        [JsonIgnore] public Agreement Agreement { get; set; }

        public int Period { get; set; }
        public string TimeUnit { get; set; }
        public string Account { get; set; }
        public int PurchaseOrderRevision { get; set; }
    }
}