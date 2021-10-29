using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
