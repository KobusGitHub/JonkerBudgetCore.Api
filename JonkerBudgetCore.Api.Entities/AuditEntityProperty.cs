namespace JonkerBudgetCore.Api.Entities
{
    public class AuditEntityProperty
    {
        public long Id { get; set; }
        public long AuditEntityId { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        // Navigation Properties
        public AuditEntity AuditEntity { get; set; }
    }
}
