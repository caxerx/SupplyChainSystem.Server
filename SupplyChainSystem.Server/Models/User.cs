using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SupplyChainSystem.Server.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required, StringLength(8)] public string UserType { get; set; }

        [Required, StringLength(256)] public string UserName { get; set; }

        [Required, StringLength(256)] public string UserPassword { get; set; }
    }
}