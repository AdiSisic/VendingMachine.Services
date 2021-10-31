using System.ComponentModel.DataAnnotations;
using VendingMachine.Services.Api.Enums;

namespace VendingMachine.Services.Api.User.Request
{
    public class CreateMemberRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
