using VendingMachine.Services.Api.Enums;

namespace VendingMachine.Services.Application.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Deposit { get; set; }
        public RoleType Role { get; set; }
    }
}
