using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChainSystem.Server.Models
{
    public class Supplier
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SupplierId { get; set; }

        [Required] public string SupplierName { get; set; }

        public string SupplierPhoneNumber { get; set; }

        public string SupplierEmail { get; set; }

        public string SupplierAddress { get; set; }

        public string SupplierContactPerson { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}