using System;
using System.Collections.Generic;

namespace JonkerBudgetCore.Api.Entities
{
    public class AuditEntity
    {
        public AuditEntity()
        {
        }        

        public AuditEntity(string user)
        {
            AuditUsername = user;
        }

        public int ActionId { get; set; }
        public string AuditUsername { get; set; }
        public string EntityId { get; set; }
        public string EntityName { get; set; }

        private DateTime? auditDateUtc;
        public DateTime AuditDateUtc
        {
            get { return auditDateUtc ?? DateTime.UtcNow; }
            set { auditDateUtc = value; }
        }

        public long Id { get; set; }

        //Navigation Properties
        public List<AuditEntityProperty> AuditEntityProperties { get; set; }
    }
}