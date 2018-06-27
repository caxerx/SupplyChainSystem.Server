using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class DespatchInstruction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int DeliveryNoteId { get; set; }

        public int DeliveryStatus { get; set; }

        [ForeignKey("Request")] public int RequestId { get; set; }

        public Request Request { get; set; }
    }
}