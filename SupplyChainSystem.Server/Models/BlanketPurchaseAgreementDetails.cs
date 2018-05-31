using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class BlanketPurchaseAgreementDetails
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }
        [Required] public int PurchaseOrderRevision { get; set; }
        [Required] public string Account { get; set; }
        [Required] public double AmountAgreed { get; set; }

        [JsonIgnore] public Agreement Agreement { get; set; }
    }
}