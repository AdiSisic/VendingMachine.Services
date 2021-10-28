using System.Collections.Generic;
using System.Threading.Tasks;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application.Abstractions
{
    public interface IRoleBll
    {
        /// <summary>
        /// Returns all Roles
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Role>> GetRoles();
    }
}
