using System;
using System.Security.Cryptography;
using System.Text;

namespace SupplyChainSystem.Server
{
    public static class HashUtilities
    {
        public static string Salt { get; set; }

        public static string HashPassword(string pw)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            var source = Encoding.Default.GetBytes(pw + Salt);
            var crypto = sha256.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }
    }
}