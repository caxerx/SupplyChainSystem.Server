using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class RequestItem
    {
        [ForeignKey("Request")] public int RequestId { get; set; }
        [JsonIgnore] public Request Request { get; set; }

        public int VirtualItemId { get; set; }
        [JsonIgnore] public VirtualItem VirtualItem { get; set; }

        public int Quantity { get; set; }

        [NotMapped] public string VirtualItemName { get; set; }
        [NotMapped] public string RequestVirtualItemId { get; set; }
    }
}