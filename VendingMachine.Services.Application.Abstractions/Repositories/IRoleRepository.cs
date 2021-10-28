using System.Collections.Generic;
using System.Threading.Tasks;
using VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Returns all Roles
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Role>> GetRoles();
    }
}
