using System.ComponentModel.DataAnnotations;

namespace JonkerBudgetCore.Api.Auth.User
{
    public class ApplicationUser
    {
        [Required]
        public string UserName { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }       
    }
}
