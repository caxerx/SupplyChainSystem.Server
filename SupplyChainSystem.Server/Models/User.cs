using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChainSystem.Server.Models
{
    public class User
    {
        public User()
        {
        }

        public User(string username, string name, string password, UserType type)
        {
            UserName = username;
            Name = name;
            Password = HashUtilities.HashPassword(password);
            UserType = type;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserId { get; set; }

        [Required] public UserType UserType { get; set; }

        [Required] public string UserName { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Password { get; set; }
    }
}