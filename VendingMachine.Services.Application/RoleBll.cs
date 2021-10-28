using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application
{
    public class RoleBll : IRoleBll
    {
        public static string AllRoles = "ALL_ROLES";

        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IRoleRepository _roleRepository;

        public RoleBll(IMapper mapper, IMemoryCache memoryCache, IRoleRepository roleRepository)
        {
            _memoryCache = memoryCache;
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            _memoryCache.TryGetValue(AllRoles, out IEnumerable<Role> roles);

            if(roles == null || roles.Count() == 0)
            {
                var dbRoles = await _roleRepository.GetRoles();

                //TODO:
                //_mapper.Map<Role, Domain.Role>(dbRoles);
            }
        }
    }
}
