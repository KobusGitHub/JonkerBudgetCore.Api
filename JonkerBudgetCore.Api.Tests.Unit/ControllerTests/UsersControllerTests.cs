using AutoMapper;
using JonkerBudgetCore.Api.Api.Controllers;
using JonkerBudgetCore.Api.Api.Providers;
using JonkerBudgetCore.Api.Api.ViewModels;
using JonkerBudgetCore.Api.Domain.Models.Roles;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Domain.Services.Roles;
using JonkerBudgetCore.Api.Domain.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JonkerBudgetCore.Api.Tests.Unit.ControllerTests
{
    /*
        Typical controller responsibilities:
        ===================================
        Verify ModelState.IsValid
        Return an error response if ModelState is invalid
        Retrieve a business entity from persistence
        Perform an action on the business entity
        Save the business entity to persistence
        Return an appropriate IActionResult
    */

    [Trait("Category", "Unit")]
    [Trait("Category", "Controller")]
    public class UsersControllerTests
    {        
        [Fact]
        public async Task GetUser_ReturnsUser_WhenValidUserId()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            

            var mockADProvider = new Mock<ActiveDirectoryProvider>();
            var mockMapper = new Mock<IMapper>();

            mockUsersService.Setup(service => service.GetUser(It.IsAny<Guid>()))
                .Returns(Task.FromResult(GetTestUser()));
            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            mockMapper.Setup(x => x.Map<UserViewModel>(It.IsAny<User>()))
               .Returns(new UserViewModel());

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.GetUser(It.IsAny<Guid>());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<UserViewModel>(okResult.Value);           
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenInvalidUserId()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();
            var mockMapper = new Mock<IMapper>();
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            mockUsersService.Setup(service => service.GetUser(It.IsAny<Guid>())).Returns(Task.FromResult(GetTestUser()));
            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(false));

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.GetUser(It.IsAny<Guid>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsAssignableFrom<Guid>(notFoundResult.Value);
        }

        [Fact]
        public async Task RegisterUser_ReturnsUser_WhenGivenValidModel_And_PolicyPassed()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockMapper = new Mock<IMapper>();
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            var testRegisterUserModel = GetTestRegisterUserModel();

            mockUsersService.Setup(service => service.Exists(It.IsAny<string>())).Returns(Task.FromResult(false));
            mockUsersService.Setup(service => service.RegisterUser(testRegisterUserModel))
                .Returns(Task.FromResult(GetTestUser()))
                .Verifiable();
            mockMapper.Setup(x => x.Map<UserViewModel>(It.IsAny<User>()))
                .Returns(new UserViewModel());
            //mockRegisterUserPolicy.Setup(policy => policy.IsValid(It.IsAny<RegisterUserModel>())).Returns(Task.FromResult(true));

            var controller = new UsersController(mockUsersService.Object,                             
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.RegisterUser(testRegisterUserModel);

            // Assert            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<UserViewModel>(okResult.Value);
        }

        [Fact]
        public async Task RegisterUser_ReturnsBadRequest_When_GivenInvalidModel()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockADProvider = new Mock<ActiveDirectoryProvider>();
            var mockMapper = new Mock<IMapper>();

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.RegisterUser(model: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RegisterDomainUser_ReturnsUser_WhenGivenValidModel_And_WhenUsernameNotInUse()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockADProvider = new Mock<ActiveDirectoryProvider>();
            var mockMapper = new Mock<IMapper>();

            var testRegisterUserModel = GetTestRegisterDomainUserModel();

            //mockRegisterDomainUserPolicy.Setup(policy => policy.IsValid(It.IsAny<RegisterDomainUserModel>())).Returns(Task.FromResult(true));
            mockUsersService.Setup(service => service.RegisterDomainUser(testRegisterUserModel))
                .Returns(Task.FromResult(GetTestUser()))
                .Verifiable();
            mockMapper.Setup(x => x.Map<UserViewModel>(It.IsAny<User>()))
                .Returns(new UserViewModel());            

            var controller = new UsersController(mockUsersService.Object,                         
                mockRolesService.Object,
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.RegisterDomainUser(testRegisterUserModel);

            // Assert            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<UserViewModel>(okResult.Value);
        }

        [Fact]
        public async Task RegisterDomainUser_ReturnsBadRequest_When_GivenInvalidModel()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockADProvider = new Mock<ActiveDirectoryProvider>();
            var mockMapper = new Mock<IMapper>();

            var controller = new UsersController(mockUsersService.Object,                           
                mockRolesService.Object,
                mockADProvider.Object,
                mockMapper.Object);

            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.RegisterDomainUser(model: null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);            
        }

        [Fact]
        public async Task AddRoleToUser_ReturnsOk()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockMapper = new Mock<IMapper>();
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            mockRolesService.Setup(service => service.Exists(It.IsAny<int>())).Returns(Task.FromResult(true));

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,            
                mockADProvider.Object,
                mockMapper.Object);
            
            // Act
            var result = await controller.AddRoleToUser(It.IsAny<Guid>(), It.IsAny<int>());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddRoleToUser_ReturnsNotFound_When_GivenInvalidUserId()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockMapper = new Mock<IMapper>();
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(false));
            mockRolesService.Setup(service => service.Exists(It.IsAny<int>())).Returns(Task.FromResult(true));

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.AddRoleToUser(It.IsAny<Guid>(), It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsAssignableFrom<Guid>(notFoundResult.Value);
        }

        [Fact]
        public async Task AddRoleToUser_ReturnsNotFound_When_GivenInvalidRoleId()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockMapper = new Mock<IMapper>();
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            mockRolesService.Setup(service => service.Exists(It.IsAny<int>())).Returns(Task.FromResult(false));

            var controller = new UsersController(mockUsersService.Object,               
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.AddRoleToUser(It.IsAny<Guid>(), It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsAssignableFrom<int>(notFoundResult.Value);
        }

        [Fact]
        public async Task RemoveRoleFromUser_ReturnsOk()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockMapper = new Mock<IMapper>();
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            mockRolesService.Setup(service => service.Exists(It.IsAny<int>())).Returns(Task.FromResult(true));

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.RemoveRoleFromUser(It.IsAny<Guid>(), It.IsAny<int>());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RemoveRoleFromUser_ReturnsNotFound_When_GivenInvalidUserId()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockMapper = new Mock<IMapper>();
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(false));
            mockRolesService.Setup(service => service.Exists(It.IsAny<int>())).Returns(Task.FromResult(true));

            var controller = new UsersController(mockUsersService.Object,                    
                mockRolesService.Object,
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.RemoveRoleFromUser(It.IsAny<Guid>(), It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsAssignableFrom<Guid>(notFoundResult.Value);
        }

        [Fact]
        public async Task RemoveRoleFromUser_ReturnsNotFound_When_GivenInvalidRoleId()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            var mockMapper = new Mock<IMapper>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            mockRolesService.Setup(service => service.Exists(It.IsAny<int>())).Returns(Task.FromResult(false));

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.AddRoleToUser(It.IsAny<Guid>(), It.IsAny<int>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsAssignableFrom<int>(notFoundResult.Value);
        }

        [Fact]
        public async Task GetRolesForUser_ReturnsRoles()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockADProvider = new Mock<ActiveDirectoryProvider>();
            var mockMapper = new Mock<IMapper>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(true));            

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.GetRolesForUser(It.IsAny<Guid>());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Role>>(okResult.Value);
        }

        [Fact]
        public async Task GetRolesForUser_ReturnsNotFound_When_GivenInvalidUserId()
        {
            // Arrange
            var mockUsersService = new Mock<IUsersService>();            
            var mockRolesService = new Mock<IRolesService>();            
            var mockADProvider = new Mock<ActiveDirectoryProvider>();

            var mockMapper = new Mock<IMapper>();

            mockUsersService.Setup(service => service.Exists(It.IsAny<Guid>())).Returns(Task.FromResult(false));

            var controller = new UsersController(mockUsersService.Object,                
                mockRolesService.Object,                
                mockADProvider.Object,
                mockMapper.Object);

            // Act
            var result = await controller.GetRolesForUser(It.IsAny<Guid>());

            // Assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsAssignableFrom<Guid>(okResult.Value);
        }

        private RegisterDomainUserModel GetTestRegisterDomainUserModel()
        {
            return new RegisterDomainUserModel
            {                
                Username = "username",                
                Roles = null
            };
        }

        private RegisterUserModel GetTestRegisterUserModel()
        {
            return new RegisterUserModel
            {
                Email = "email@email.com",
                Username = "username",
                Password = "password",
                Roles = null
            };
        }

        private User GetTestUser()
        {
            return new User
            {
                UserId = Guid.NewGuid(),
                Username = "TestUser"
            };
        }
    }
}
