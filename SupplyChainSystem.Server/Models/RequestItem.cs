using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SupplyChainSystem.Server.Models
{
    public class RequestItem
    {
        [Key] [ForeignKey("Request")] public int RequestId { get; set; }
        public Request Request { get; set; }

        public int VirtualItemId { get; set; }
        public VirtualItem VirtualItem { get; set; }

        public int Quantity { get; set; }
    }
}