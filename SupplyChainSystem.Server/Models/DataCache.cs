using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class DataCache
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int DataCacheId { get; set; }

        public DateTime CacheTime { get; set; }

        public String CacheType { get; set; }

        [Column(TypeName = "longtext")] public string Content { get; set; }
    }
}