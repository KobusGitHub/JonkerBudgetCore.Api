using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Persistence
{
    public class ApplicationAuditDbContext : DbContext
    {
        private readonly IUserInfoProvider userInfoProvider;        

        public ApplicationAuditDbContext(DbContextOptions<ApplicationDbContext> options,
            IUserInfoProvider userInfoProvider) : base(options)
        {            
            this.userInfoProvider = userInfoProvider;
        }

        public ApplicationAuditDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {            
        }

        public DbSet<AuditEntity> AuditEntities { get; set; }
        public DbSet<AuditEntityProperty> AuditEntityProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // AuditEntity
            modelBuilder.Entity<AuditEntity>()
                .HasKey(key => key.Id);
            modelBuilder.Entity<AuditEntity>()
                .ToTable("AuditEntity");
            modelBuilder.Entity<AuditEntity>()
                .Property(p => p.AuditUsername)
                .HasColumnType("varchar(250)");
            modelBuilder.Entity<AuditEntity>()
                .Property(p => p.EntityName)
                .HasColumnType("varchar(250)");
            modelBuilder.Entity<AuditEntity>()
                .Property(p => p.EntityId)
                .HasColumnType("varchar(50)");

            // AuditEntityProperty
            modelBuilder.Entity<AuditEntityProperty>()
                .HasKey(key => key.Id);
            modelBuilder.Entity<AuditEntityProperty>()
                .ToTable("AuditEntityProperty")
            .HasOne(ac => ac.AuditEntity)
                .WithMany(m => m.AuditEntityProperties)
                .IsRequired()
                .HasForeignKey(fk => fk.AuditEntityId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuditEntityProperty>()
                .Property(p => p.NewValue)
                .HasColumnType("nvarchar(MAX)");
            modelBuilder.Entity<AuditEntityProperty>()
                .Property(p => p.OldValue)
                .HasColumnType("nvarchar(MAX)");
            modelBuilder.Entity<AuditEntityProperty>()
                .Property(p => p.PropertyName)
                .HasColumnType("varchar(250)");
        }

        private string GetUsername()
        {
            var username = "Anonymous";
            if (userInfoProvider != null)
                username = userInfoProvider.Username;

            return username;
        }

        private static void UpdateAddedEntities(List<Tuple<EntityEntry, AuditEntity, string>> audits)
        {
            // Update added entities   
            foreach (var inserted in audits)
            {
                string id = inserted.Item1.CurrentValues[inserted.Item3].ToString();
                inserted.Item2.EntityId = id;
            }
        }

        public override int SaveChanges()
        {
            List<Tuple<EntityEntry, AuditEntity, string>> audits = BuildUpAudits(GetUsername());
            var i = base.SaveChanges();

            UpdateAddedEntities(audits);

            if (audits.Count > 0)
            {
                int j = SaveChanges();
                return i + j;
            }

            return i;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Tuple<EntityEntry, AuditEntity, string>> audits = BuildUpAudits(GetUsername());
            var i = await base.SaveChangesAsync(cancellationToken);

            UpdateAddedEntities(audits);

            if (audits.Count > 0)
            {
                int j = await SaveChangesAsync();
                return i + j;
            }

            return i;
        }

        private List<Tuple<EntityEntry, AuditEntity, string>> BuildUpAudits(string username)
        {
            var audits = new List<Tuple<EntityEntry, AuditEntity, string>>();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                var type = entry.Entity.GetType();
                var isAudit = type.GetTypeInfo().GetCustomAttribute<Auditable>();

                if (isAudit != null)
                {
                    var auditEntity = new AuditEntity(username);

                    if (entry.State == EntityState.Added)
                    {
                        audits.Add(new Tuple<EntityEntry, AuditEntity, string>(entry, auditEntity, isAudit.EntityIdPropertyName));
                    }

                    BuildAuditForEntity(entry, isAudit, username, auditEntity);
                }
            }

            return audits;
        }

        private void BuildAuditForEntity(EntityEntry entry,
          Auditable isAudit,
          string username,
          AuditEntity auditEntity)
        {
            string id = entry.CurrentValues[isAudit.EntityIdPropertyName].ToString();

            var list = new List<AuditEntityProperty>();
            var type = entry.Entity.GetType();

            auditEntity.ActionId = (int)entry.State;
            auditEntity.EntityName = type.Name;
            auditEntity.EntityId = id;

            foreach (var property in entry.CurrentValues.Properties)
            {
                var curVal = entry.CurrentValues[property.Name];
                object oldVal = null;

                if (entry.State == EntityState.Modified)
                    oldVal = entry.OriginalValues[property.Name];

                if (NotEqual(oldVal, curVal))
                {
                    list.Add(new AuditEntityProperty
                    {
                        PropertyName = property.Name,
                        OldValue = oldVal == null ? null : oldVal.ToString(),
                        NewValue = curVal == null ? null : curVal.ToString(),
                        AuditEntity = auditEntity,
                    });
                }
            }

            auditEntity.AuditEntityProperties = list;
            AuditEntities.Add(auditEntity);
        }

        private bool NotEqual(object oldVal, object curVal)
        {
            if (oldVal == null && curVal != null)
                return true;
            if (oldVal != null && curVal == null)
                return true;
            if (oldVal == null && curVal == null)
                return false;

            var oldValType = oldVal.GetType().FullName;
            var curValType = oldVal.GetType().FullName;

            var oldValue = Convert.ChangeType(oldVal, Type.GetType(oldValType));
            var curValue = Convert.ChangeType(curVal, Type.GetType(curValType));

            if (oldValue.ToString() != curValue.ToString())
                return true;

            return false;
        }
    }
}
