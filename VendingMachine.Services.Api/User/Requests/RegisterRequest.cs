using VendingMachine.Services.Api.Enums;

namespace VendingMachine.Services.Api.User.Request
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Deposit { get; set; }
        public Role Role { get; set; }
    }
}
