using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.ResponseWrapper
{
    public class AgreementResponseWrapper
    {
        public int AgreementType { get; set; }
        public string Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Supplier { get; set; }
        public dynamic Details { get; set; }
        public ICollection<dynamic> Items { get; set; }
    }
}