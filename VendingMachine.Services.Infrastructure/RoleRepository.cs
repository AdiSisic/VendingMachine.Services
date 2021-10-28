using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Domain;
using VendingMachine.Services.Infrastructure.EFDbContext;

namespace VendingMachine.Services.Infrastructure
{
    public class RoleRepository : IRoleRepository
    {
        public async Task<IEnumerable<Role>> GetRoles()
        {
            using VendingMachineContext context = new();
            return await context.Roles.ToListAsync();
        }
    }
}
