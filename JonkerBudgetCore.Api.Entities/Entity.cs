using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JonkerBudgetCore.Api.Entities
{
    public interface IModifiableEntity
    {
        DateTime CreatedDateUtc { get; set; }
        DateTime? LastModifiedDateUtc { get; set; }
        string CreatedBy { get; set; }
        string LastModifiedBy { get; set; }
        bool IsActive { get; set; }
    }

    public abstract class Entity : IModifiableEntity
    {
        public Entity()
        {
        }

        public Entity(string user)
        {
            CreatedBy = user;
        }        

        private DateTime? createdDateUtc;        
        public DateTime CreatedDateUtc
        {
            get { return createdDateUtc ?? DateTime.UtcNow; }
            set { createdDateUtc = value; }
        }
        public DateTime? LastModifiedDateUtc { get; set; }

        [Column(TypeName = "varchar(100)")]        
        public string CreatedBy { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string LastModifiedBy { get; set; }
        private bool? isActive;
        public bool IsActive
        {
            get { return isActive ?? true; }
            set { isActive = value; }
        }
    }
}
