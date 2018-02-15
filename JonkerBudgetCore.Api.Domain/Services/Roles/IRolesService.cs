using JonkerBudgetCore.Api.Domain.Models.Roles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Services.Roles
{
    public interface IRolesService
    {
        Task<IEnumerable<Role>> GetRoles();
        Task<IEnumerable<Role>> GetRolesForUser(Guid userId);
        Task<bool> Exists(int roleId);
        Task AddRoleToUser(Guid userId, int roleId);
        Task RemoveRoleFromUser(Guid userId, int roleId);
    }
}
