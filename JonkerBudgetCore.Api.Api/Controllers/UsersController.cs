using AutoMapper;
using JonkerBudgetCore.Api.Api.ViewModels;
using JonkerBudgetCore.Api.Api.ViewModels.Users;
using JonkerBudgetCore.Api.Auth.ActiveDirectory;
using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Domain.Services.Roles;
using JonkerBudgetCore.Api.Domain.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]

    public class UsersController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IRolesService rolesService;
        private readonly IMapper mapper;
        private IActiveDirectoryProvider activeDirectoryUserProvider;

        public UsersController(IUsersService usersService,
            IRolesService rolesService,
            IActiveDirectoryProvider activeDirectoryUserProvider,
            IMapper mapper)
        {
            this.usersService = usersService;
            this.rolesService = rolesService;
            this.activeDirectoryUserProvider = activeDirectoryUserProvider;
            this.mapper = mapper;
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            if (!await usersService.Exists(userId))
            {
                return NotFound(userId);
            }

            var user = await usersService.GetUser(userId);

            return Ok(mapper.Map<UserViewModel>(user));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await usersService.GetUsers();

            return Ok(mapper.Map<IEnumerable<UserViewModel>>(users));
        }

        [HttpGet("GetUsersWithEnabledRoles")]
        [Authorize]
        public async Task<IActionResult> GetUsersWithEnabledRoles()
        {
            var users = await usersService.GetUsersWithEnabledRoles();

            return Ok(users);
        }

        [HttpGet("Query/{query}")]
        [Authorize]
        public async Task<IActionResult> LookupUsers(string query)
        {
            var users = await usersService.LookupUsers(query);

            return Ok(mapper.Map<IEnumerable<UserViewModel>>(users));
        }


        [HttpGet("QueryAD/{username}")]
        public async Task<IActionResult> LookupUsersActiveDirectory(string username)
        {
            var users = await Task.FromResult(activeDirectoryUserProvider.QueryActiveDirectory(username));

            return Ok(mapper.Map<IEnumerable<ActiveDirectoryUserViewModel>>(users).OrderBy(s => s.Firstname));
        }

        [HttpGet("{userId}/Roles")]
        [Authorize]
        public async Task<IActionResult> GetRolesForUser(Guid userId)
        {
            if (!await usersService.Exists(userId))
            {
                return NotFound(userId);
            }

            var roles = await rolesService.GetRolesForUser(userId);

            return Ok(roles);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, UserManagement")]
        public async Task<IActionResult> RegisterUser([FromBody]RegisterUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await usersService.RegisterUser(model);
            var userModel = mapper.Map<UserViewModel>(user);

            return Ok(userModel);
        }

        [HttpPost("Domain")]
        [Authorize(Roles = "Administrator, UserManagement")]
        public async Task<IActionResult> RegisterDomainUser([FromBody]RegisterDomainUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await usersService.RegisterDomainUser(model);
            var userModel = mapper.Map<UserViewModel>(user);

            return Ok(userModel);
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, UserManagement")]
        public async Task<IActionResult> EditUser([FromBody]UpdateUserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var returnedUser = new User();
            var returnedUserViewModel = new UserViewModel();

            try
            {
                returnedUser = await this.usersService.UpdateUser(user);
                returnedUserViewModel = mapper.Map<UserViewModel>(returnedUser);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            if (returnedUser == null || returnedUserViewModel == null)
                return BadRequest();

            return Ok(returnedUserViewModel);
        }

        [HttpPut("{userId}/Roles/{roleId}")]
        [Authorize(Roles = "Administrator, UserManagement")]
        public async Task<IActionResult> AddRoleToUser(Guid userId, int roleId)
        {
            if (!await usersService.Exists(userId))
            {
                return NotFound(userId);
            }

            if (!await rolesService.Exists(roleId))
            {
                return NotFound(roleId);
            }

            await rolesService.AddRoleToUser(userId, roleId);

            return Ok();
        }

        [HttpDelete("{userId}/Roles/{roleId}")]
        [Authorize(Roles = "Administrator, UserManagement")]
        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, int roleId)
        {
            if (!await usersService.Exists(userId))
            {
                return NotFound(userId);
            }

            if (!await rolesService.Exists(roleId))
            {
                return NotFound(roleId);
            }

            await rolesService.RemoveRoleFromUser(userId, roleId);

            return Ok();
        }

        [HttpPost("RequestPasswordReset")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody]PasswordResetRequestModel model)
        {
            var result = await usersService.RequestPasswordReset(model);

            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordModel model)
        {
            var result = await usersService.ResetPassword(model);

            return Ok(result); 
        }
    }
}