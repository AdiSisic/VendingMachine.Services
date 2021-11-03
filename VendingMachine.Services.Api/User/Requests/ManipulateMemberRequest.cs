using System.ComponentModel.DataAnnotations;
using VendingMachine.Services.Api.Enums;

namespace VendingMachine.Services.Api.User.Request
{
    public class ManipulateMemberRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public RoleType Role { get; set; }
    }
}
