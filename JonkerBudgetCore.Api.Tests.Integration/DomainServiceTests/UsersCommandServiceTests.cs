using JonkerBudgetCore.Api.Auth.Encrypt;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Domain.Services.Users;
using JonkerBudgetCore.Api.Persistence;
using JonkerBudgetCore.Api.Tests.Integration.Mocks;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Moq;
using Xunit;
using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Shared_Services;
using Microsoft.Extensions.Configuration;

namespace JonkerBudgetCore.Api.Tests.Integration.DomainServiceTests
{

    [Trait("Category", "Integration")]
    [Trait("Category", "Service")]
    public class UsersCommandServiceTests
    {
        //IPolicy<RegisterUserModel> registerUserPolicy;
        //IPolicy<RegisterDomainUserModel> registerDomainUserPolicy;
        //IPolicy<UpdateUserModel> updateUserPolicy;

        [Fact]
        public async Task RegisterUser_Creates_And_Returns_User()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            var tester = "Tester";
            var encrypter = new Encrypter();
            var userInfoProvider = new MockUserInfoProvider(tester);

            var mockRegisterUserPolicy = new Mock<IPolicy<RegisterUserModel>>();
            var mockRegisterDomainUserPolicy = new Mock<IPolicy<RegisterDomainUserModel>>();
            var updateUserPolicy = new Mock<IPolicy<UpdateUserModel>>();
            
            var mockResetPasswordPolicy = new Mock<IPolicy<ResetPasswordModel>>();
            var mockPasswordResetRequestPolicy = new Mock<IPolicy<PasswordResetRequestModel>>();
            var mockPasswordPolicy = new Mock<IPolicy<PasswordModel>>();
            var mockCommsService = new Mock<ICommsService>();
            var mockConfigurationRoot = new Mock<IConfigurationRoot>();

            var newUser = new RegisterUserModel
            {
                Email = "email@email.com",
                Password = "TestPassword@123",
                Username = "TestUser"
            };

            mockRegisterUserPolicy.Setup(service => service.IsValid(It.IsAny<RegisterUserModel>())).Returns(Task.FromResult(true));

            // Make sure there is no user with username
            using (var context = new ApplicationDbContext(options, userInfoProvider))
            {
                // PreAssert
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == newUser.Username);
                Assert.Null(user);
            }

            // Register user through domain service
            using (var context = new ApplicationDbContext(options, userInfoProvider))
            {
                // Arrange
                var service = new UsersService(context, 
                    encrypter, 
                    userInfoProvider,
                    mockRegisterUserPolicy.Object,
                    mockRegisterDomainUserPolicy.Object,
                    updateUserPolicy.Object,
                    mockPasswordResetRequestPolicy.Object,
                    mockResetPasswordPolicy.Object,
                    mockPasswordPolicy.Object,
                    mockCommsService.Object,
                    mockConfigurationRoot.Object
                    );

                // Act
                var registeredUser = (await service.RegisterUser(newUser));                

                Assert.NotNull(registeredUser);
            }

            // Check that domain user was registered
            using (var context = new ApplicationDbContext(options, userInfoProvider))
            {
                var dbUser = await context
                    .Users
                    .FirstOrDefaultAsync(u => u.Username == newUser.Username);

                // Assert
                Assert.NotNull(dbUser);
                Assert.Equal(newUser.Username, dbUser.Username);
                Assert.Equal(newUser.Email, dbUser.Email);
                Assert.NotEqual(newUser.Password, dbUser.PasswordHash);
                Assert.Equal(tester, dbUser.CreatedBy);
            }
        }
    }
}