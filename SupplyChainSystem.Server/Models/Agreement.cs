using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SupplyChainSystem.Server.Models
{
    public class Agreement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AgreementId { get; set; }

        public string Currency { get; set; }

        [Required] public int SupplierId { get; set; }
        [Required] public AgreementType AgreementType { get; set; }
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime ExpiryDate { get; set; }


        [Required] [ForeignKey("User")] public int CreateBy { get; set; }
        [JsonIgnore] public User User { get; set; }
        [JsonIgnore] public Supplier Supplier { get; set; }
    }
}