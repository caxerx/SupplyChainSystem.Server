using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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

        [MaxLength(20)] [Required] public string UserName { get; set; }

        [Required] public string Name { get; set; }

        [Required] [JsonIgnore] public string Password { get; set; }

        [JsonIgnore] public RestaurantManager RestaurantManager { get; set; }
    }
}