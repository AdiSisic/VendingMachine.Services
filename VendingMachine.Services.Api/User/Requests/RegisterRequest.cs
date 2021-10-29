using System.ComponentModel.DataAnnotations;
using VendingMachine.Services.Api.Enums;

namespace VendingMachine.Services.Api.User.Request
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int Deposit { get; set; }
        public Role Role { get; set; }
    }
}
