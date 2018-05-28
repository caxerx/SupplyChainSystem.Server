using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.Models
{
    public class Agreement
    {
        [Key]
        [MaxLength(20)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AgreementId { get; set; }

        [Required] public AgreementType AgreementType { get; set; }
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime ExpiryDate { get; set; }

        public BlanketPurchaseAgreementDetails BlanketPurchaseAgreementDetails { get; set; }
        public ICollection<BlanketPurchaseAgreementLine> BlanketPurchaseAgreementLines { get; set; }
    }
}