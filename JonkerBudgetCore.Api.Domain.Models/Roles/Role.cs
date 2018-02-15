using JonkerBudgetCore.Api.Entities;

namespace JonkerBudgetCore.Api.Domain.Models.Roles
{
    public class Role : Entity
    {
        public Role() {}
        public Role(string user) 
            : base(user) {}

        public int Id { get; set; }                
        public string Name { get; set; }        
        public string Description { get; set; }         
    }
}
