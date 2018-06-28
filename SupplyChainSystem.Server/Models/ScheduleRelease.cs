using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class ScheduleRelease
    {
        [Key] public int OrderId { get; set; }

        [ForeignKey("Agreement")] public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }

        public DateTime ExpectedDeliveryDate { get; set; }

        public DateTime CreateTime { get; set; }
    }
}