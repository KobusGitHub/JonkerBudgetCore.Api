using System;

namespace JonkerBudgetCore.Api.Entities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class Auditable : Attribute
    {
        public string EntityIdPropertyName { get; set; }
        public Auditable(string entityIdPropertyName)
        {
            EntityIdPropertyName = entityIdPropertyName;
        }
    }
}
