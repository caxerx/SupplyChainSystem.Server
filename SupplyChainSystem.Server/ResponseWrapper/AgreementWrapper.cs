using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.ResponseWrapper
{
    public class AgreementWrapper
    {
        public int AgreementType { get; set; }
        public string Currency { get; set; }
        public long StartDate { get; set; }
        public long ExpiryDate { get; set; }
        public int SupplierId { get; set; }
        public dynamic Details { get; set; }
        public ICollection<dynamic> Items { get; set; }
    }
}