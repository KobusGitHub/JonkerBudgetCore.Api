using JonkerBudgetCore.Api.Api.Controllers;
using JonkerBudgetCore.Api.Domain.Models.Roles;
using JonkerBudgetCore.Api.Domain.Services.Roles;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
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
    public class RolesControllerTests
    {
        [Fact]
        public async Task GetAllRoles_Returns_Collection_Of_Roles()
        {
            // Arrange
            var mockService = new Mock<IRolesService>();
            mockService.Setup(service => service.GetRoles()).Returns(Task.FromResult(GetTestRoles().AsEnumerable()));
            var controller = new RolesController(mockService.Object);

            // Act
            var result = await controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dataResult = Assert.IsAssignableFrom<IEnumerable<Role>>(okResult.Value);
            Assert.Equal(2, dataResult.Count());
        }

        private List<Role> GetTestRoles()
        {
            var roles = new List<Role>
            {
                new Role()
                {
                    Id = 1,
                    Name = "Role1"
                },
                new Role()
                {
                    Id = 2,
                    Name = "Role2"
                }
            };
            return roles;
        }
    }
}
