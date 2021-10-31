using VendingMachine.Services.Api.Enums;

namespace VendingMachine.Services.Api.User.Response
{
    public class GetUserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Deposit { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
    }
}
