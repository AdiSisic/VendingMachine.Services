using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Services.Api.User.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool KeepLogged { get; set; }
    }
}
