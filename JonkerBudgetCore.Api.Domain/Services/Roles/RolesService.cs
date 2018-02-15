using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Models.Roles;
using JonkerBudgetCore.Api.Domain.Models.UserRoles;
using JonkerBudgetCore.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Services.Roles
{
    public class RolesService : IRolesService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserInfoProvider userInfoProvider;

        public RolesService(ApplicationDbContext dbContext,
             IUserInfoProvider userInfoProvider)
        {
            this.dbContext = dbContext;
            this.userInfoProvider = userInfoProvider;
        }

        public async Task<bool> Exists(int roleId)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

            return role == null ? false : true;
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await dbContext.Roles.ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetRolesForUser(Guid userId)
        {
            return (await dbContext
                .Users
                .Include("UserRoles.Role")
                .FirstOrDefaultAsync(u => u.UserId == userId))
                .UserRoles
                .Select(p => p.Role);
        }

        public async Task AddRoleToUser(Guid userId, int roleId)
        {
            var user = await dbContext
                        .Users
                        .Include("UserRoles.Role")
                        .FirstAsync(u => u.UserId == userId);

            if (user.UserRoles.FirstOrDefault(r => r.RoleId == roleId) == null)
            {
                dbContext.UserRoles.Add(new UserRole(userInfoProvider.Username)
                {
                    UserId = userId,
                    RoleId = roleId
                });

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveRoleFromUser(Guid userId, int roleId)
        {
            var user = await dbContext
                        .Users
                        .Include("UserRoles.Role")
                        .FirstAsync(u => u.UserId == userId);

            var userRole = user.UserRoles.FirstOrDefault(r => r.RoleId == roleId);
            if (userRole != null)
            {
                dbContext.UserRoles.Remove(userRole);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
