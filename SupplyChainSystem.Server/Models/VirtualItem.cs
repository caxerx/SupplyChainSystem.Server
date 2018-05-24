﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupplyChainSystem.Server.Models
{
    public class VirtualItem
    {
        [Key] [MaxLength(20)] public string VirtualItemId { get; set; }
        [Required] public string VirtualItemName { get; set; }
        public string VirtualItemDescription { get; set; }

        public ICollection<VirtualIdMap> VirtualIdMap { get; set; }
        public ICollection<CategoryItem> CategoryItems { get; set; }
    }
}