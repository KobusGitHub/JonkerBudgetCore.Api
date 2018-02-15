using JonkerBudgetCore.Api.Domain.Models.Roles;
using JonkerBudgetCore.Api.Domain.Models.UserRoles;
using JonkerBudgetCore.Api.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Models.Widgets;
using JonkerBudgetCore.Api.Domain.Models.Dashboards;
using JonkerBudgetCore.Api.Domain.Models.WidgetQueries;
using SGStatus.WebApi.Persistence.Services;
using JonkerBudgetCore.Api.Domain.Models.UserDashboards;
using JonkerBudgetCore.Api.Domain.Models.UserLogins;
using System.Linq;
using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Models.Categories;
using JonkerBudgetCore.Api.Domain.Models.Expenses;

namespace JonkerBudgetCore.Api.Persistence
{
    public class ApplicationDbContext : ApplicationAuditDbContext
    {
        private ISqlService sqlService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ISqlService sqlService)
            : base(options)
        {
            this.sqlService = sqlService;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IUserInfoProvider userInfoProvider)
            : base(options, userInfoProvider)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<WidgetQuery> WidgetQueries { get; set; }
        public DbSet<UserDashboard> UserDashboards { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }


        public ISqlService SqlService
        {
            get
            {
                return sqlService;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Here is an alternative to the long winded approach below
            ////Prevent Cascading Deletes
            //foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            ////Set all strings as VARCHAR as opposed to NVARCHAR. Also applies a standard max length of 200.
            //foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            //{
            //    if (!property.GetMaxLength().HasValue)
            //        property.SetMaxLength(200);

            //    property.IsUnicode(false);
            //}

            // Role
            modelBuilder.Entity<Role>()
            .HasKey(key => key.Id);
            modelBuilder.Entity<Role>()
                .ToTable("Role");
            modelBuilder.Entity<Role>()
                .Property(p => p.Name)
                .HasColumnType("varchar(50)")
                .IsRequired();
            modelBuilder.Entity<Role>()
                .Property(p => p.Description)
                .HasColumnType("varchar(100)")
                .IsRequired();

            // User
            modelBuilder.Entity<User>()
                .HasKey(key => key.UserId);
            modelBuilder.Entity<User>()
                .ToTable("User");
            modelBuilder.Entity<User>()
               .Property(p => p.Username)
               .HasColumnType("varchar(100)")
               .IsRequired();
            modelBuilder.Entity<User>()
                .Property(p => p.Firstname)
                .HasColumnType("varchar(50)");
            modelBuilder.Entity<User>()
                .Property(p => p.Email)
                .HasColumnType("varchar(100)");
            modelBuilder.Entity<User>()
                .Property(p => p.Lastname)
                .HasColumnType("varchar(50)");

            // UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(key => new { key.UserId, key.RoleId });
            modelBuilder.Entity<UserRole>()
                .ToTable("UserRole");
            modelBuilder.Entity<UserRole>()
                .HasOne(c => c.Role)
                .WithMany()
                .IsRequired()
                .HasForeignKey(fk => fk.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserRole>()
                .HasOne(c => c.User)
                .WithMany(u => u.UserRoles)
                .IsRequired()
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
