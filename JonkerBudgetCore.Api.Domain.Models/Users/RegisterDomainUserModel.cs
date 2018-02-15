using System.ComponentModel.DataAnnotations;

namespace JonkerBudgetCore.Api.Domain.Models.Users
{
    public class RegisterDomainUserModel
    {
        [Required]
        public string Username { get; set; }        
        public int[] Roles { get; set; }        
    }
}
