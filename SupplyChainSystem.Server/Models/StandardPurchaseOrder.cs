using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class StandardPurchaseOrder
    {
        [Key] public int OrderId { get; set; }

        [ForeignKey("Request")] public int RequestId { get; set; }
        [ForeignKey("Agreement")] public int AgreementId { get; set; }

        public Request Request { get; set; }
        public Agreement Agreement { get; set; }

    }
}