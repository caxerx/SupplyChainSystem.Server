using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server
{
    public static class HashUtilities
    {
        public static string Salt { get; set; }

        public static string HashPassword(string pw)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] source = Encoding.Default.GetBytes(pw + Salt);
            byte[] crypto = sha256.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }
    }
}