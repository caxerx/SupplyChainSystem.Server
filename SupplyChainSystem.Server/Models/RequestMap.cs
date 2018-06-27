using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class RequestMap
    {
        public MapType MapType { get; set; }

        [Key] [ForeignKey("Request")] public int RequestId { get; set; }
        [ForeignKey("Agreement")] public int? AgreementId { get; set; }
        [ForeignKey("DespatchInstruction")] public int? DespatchInstructionId { get; set; }


        public Request Request { get; set; }
        public Agreement Agreement { get; set; }
        public DespatchInstruction DespatchInstruction { get; set; }
    }
}