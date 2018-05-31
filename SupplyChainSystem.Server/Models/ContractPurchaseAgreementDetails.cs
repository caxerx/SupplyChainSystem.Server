using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class ContractPurchaseAgreementDetails
    {
        [Key] [ForeignKey("Agreement")] public int AgreementId { get; set; }

        public string Account { get; set; }

        [JsonIgnore] public Agreement Agreement { get; set; }


    }
}