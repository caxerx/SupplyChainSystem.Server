﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SupplyChainSystem.Server.Models
{
    public class User
    {
        public User() { }
        public User(string username, string password, string type)
        {
            UserName = username;
            Password = HashUtilities.HashPassword(password);
            UserType = type;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserId { get; set; }

        [Required] public string UserType { get; set; }

        [Required] public string UserName { get; set; }

        [Required] public string Password { get; set; }
    }
}

