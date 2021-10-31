using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface IUserService
    {
        Task<BaseResponse<User>> GetUserAsync(int userId);
    }
}
