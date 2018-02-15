using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policy.Users;
using JonkerBudgetCore.Api.Domain.Repositories.Users;
using JonkerBudgetCore.Api.Domain.Services.Users;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JonkerBudgetCore.Api.Tests.Unit.DomainPolicyTests
{
    [Trait("Category", "Unit")]
    [Trait("Category", "Policy")]
    public class UsersPolicyTests
    {
        [Fact]
        public async Task PasswordPolicy_Validates_Correct_Password()
        {
            // Arrange
            var passwordPolicy = new PasswordPolicy();

            // Act
            var result = await passwordPolicy.IsValid(new PasswordModel() { Password = "APassword@123" });
            var policyViolations = passwordPolicy.PolicyViolations.ToList();

            // Assert
            Assert.True(result);
            Assert.Equal(0, policyViolations.Count);
        }

        [Fact]
        public async Task PasswordPolicy_Invalidates_Incorrect_Length_But_Valid_Caps_And_Numeric()
        {
            // Arrange
            var passwordPolicy = new PasswordPolicy();

            // Act
            var result = await passwordPolicy.IsValid(new PasswordModel { Password = "Ab1" });
            var policyViolations = passwordPolicy.PolicyViolations.ToList();

            // Assert
            Assert.False(result);
            Assert.Equal(1, policyViolations.Count);
            Assert.Equal("PasswordLength", policyViolations.First().Key);            
        }

        [Fact]
        public async Task PasswordPolicy_Invalidates_Correct_Length_But_Invalid_Caps_And_Valid_Numeric()
        {
            // Arrange
            var passwordPolicy = new PasswordPolicy();

            // Act
            var result = await passwordPolicy.IsValid(new PasswordModel { Password = "abcde1" });
            var policyViolations = passwordPolicy.PolicyViolations.ToList();

            // Assert
            Assert.False(result);
            Assert.Equal(1, policyViolations.Count);
            Assert.Equal("PasswordComplexity", policyViolations.First().Key);
        }

        [Fact]
        public async Task PasswordPolicy_Invalidates_Correct_Length_But_Valid_Caps_And_Invalid_Numeric()
        {
            // Arrange
            var passwordPolicy = new PasswordPolicy();

            // Act
            var result = await passwordPolicy.IsValid(new PasswordModel { Password = "Abcdef" });
            var policyViolations = passwordPolicy.PolicyViolations.ToList();

            // Assert
            Assert.False(result);
            Assert.Equal(1, policyViolations.Count);
            Assert.Equal("PasswordComplexity", policyViolations.First().Key);
        }

        [Fact]
        public async Task UsernamePolicy_Validates_Username_Not_In_Use()
        {
            var mockUserRepository = new Mock<IUsersRepository>();

            // Arrange
            mockUserRepository.Setup(service => service.Exists(It.IsAny<string>())).Returns(Task.FromResult(false));

            var usernamePolicy = new UsernamePolicy(mockUserRepository.Object);

            // Act
            var result = await usernamePolicy.IsValid(new UsernameModel { Username = "username" });
            var policyViolations = usernamePolicy.PolicyViolations.ToList();

            // Assert
            Assert.True(result);
            Assert.Equal(0, policyViolations.Count);            
        }

        [Fact]
        public async Task UsernamePolicy_Invalidates_Username_Not_In_Use()
        {
            var mockUsersRepository = new Mock<IUsersRepository>();

            // Arrange
            mockUsersRepository.Setup(service => service.Exists(It.IsAny<string>())).Returns(Task.FromResult(true));

            var usernamePolicy = new UsernamePolicy(mockUsersRepository.Object);

            // Act
            var result = await usernamePolicy.IsValid(new UsernameModel { Username = "username" });
            var policyViolations = usernamePolicy.PolicyViolations.ToList();

            // Assert
            Assert.False(result);
            Assert.Equal(1, policyViolations.Count);
            Assert.Equal("UsernameInUse", policyViolations.First().Key);
        }
    }
}
