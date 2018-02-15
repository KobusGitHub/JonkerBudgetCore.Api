using System;
using System.ComponentModel.DataAnnotations;

namespace JonkerBudgetCore.Api.Domain.Models.Users
{
    public class UpdateUserModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public bool IsAdUser { get; set; }

        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public int[] Roles { get; set; }
        public Guid UserId { get; set; }
    }
}
