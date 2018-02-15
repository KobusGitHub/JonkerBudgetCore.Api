using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Models.Roles;
using JonkerBudgetCore.Api.Domain.Services.Roles;
using JonkerBudgetCore.Api.Persistence;
using JonkerBudgetCore.Api.Tests.Integration.Mocks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JonkerBudgetCore.Api.Tests.Integration.DomainServiceTests
{
    [Trait("Category", "Integration")]
    [Trait("Category", "Service")]
    public class RolesQueryServiceTests
    {
        [Fact]        
        public async Task GetAllRoles_Returns_Roles()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            var mockUserInfoProvider = new MockUserInfoProvider("Test User");

            // Setup some data
            using (var context = new ApplicationDbContext(options, mockUserInfoProvider))
            {
                context.Roles.Add(new Role("TestUser")
                {
                    Name = "SomeRole",
                    Description = "SomeRole"                    
                });
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new ApplicationDbContext(options, mockUserInfoProvider))
            {      
                // Arrange
                var service = new RolesService(context, mockUserInfoProvider);

                // Act
                var roles = (await service.GetRoles()).ToList();

                // Assert
                Assert.Equal(1, roles.Count);
                Assert.Equal("SomeRole", roles.First().Name);
                Assert.Equal("TestUser", roles.First().CreatedBy);
            }               
        }
    }
}
