﻿using System;
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
        [Key] public int DespatchInstructionId { get; set; }

        public int DespatchInstructionStatus { get; set; }

        [ForeignKey("Request")] public int RequestId { get; set; }

        public DateTime CreateTime { get; set; }

        public Request Request { get; set; }
    }
}