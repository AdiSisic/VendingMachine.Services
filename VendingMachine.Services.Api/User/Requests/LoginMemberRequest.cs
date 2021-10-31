using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Services.Api.User.Requests
{
    public class LoginMemberRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
