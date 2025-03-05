using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Models;

namespace TechSupportXPress.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketSubCategory> TicketSubCategories { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeDetail> SystemCodeDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<TicketResolution> TicketResolutions { get; set; }

        public DbSet<TicketsSummaryView> TicketsSummaryView { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TicketsSummaryView>()
                .HasNoKey()
                .ToTable(nameof(TicketsSummaryView), k => k.ExcludeFromMigrations());

            builder.Entity<TicketResolution>()
            .HasOne(c => c.Status)
            .WithMany()
            .HasForeignKey(c => c.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketResolution>()
           .HasOne(c => c.Ticket)
           .WithMany()
           .HasForeignKey(c => c.TicketId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
        .HasOne(c => c.Priority)
        .WithMany()
        .HasForeignKey(c => c.PriorityId)
        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SystemCodeDetail>()
         .HasOne(c => c.SystemCode)
         .WithMany()
         .HasForeignKey(c => c.SystemCodeId)
         .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketCategory>()
            .HasOne(c => c.DeletedBy)
            .WithMany()
            .HasForeignKey(c => c.DeletedById)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketCategory>()
            .HasOne(c => c.ModifiedBy)
            .WithMany()
            .HasForeignKey(c => c.ModifiedById)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketCategory>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
           .HasOne(c => c.Ticket)
           .WithMany(c => c.TicketComments)
           .HasForeignKey(c => c.TicketId)
           .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Ticket>()
                   .HasOne(c => c.CreatedBy)
                   .WithMany()
                   .HasForeignKey(c => c.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);
        }

        public void EnsureViewsCreated()
        {
            var createViewSql = @"
        CREATE OR REPLACE VIEW TicketsSummaryView AS
        SELECT 
            COALESCE(COUNT(T1.Id), 0) AS TotalTickets,
            COALESCE(SUM(CASE WHEN T2.Code = 'Assigned' THEN 1 ELSE 0 END), 0) AS AssignedTickets,
            COALESCE(SUM(CASE WHEN T2.Code = 'Closed' THEN 1 ELSE 0 END), 0) AS ClosedTickets,
            COALESCE(SUM(CASE WHEN T2.Code = 'Pending' THEN 1 ELSE 0 END), 0) AS PendingTickets,
            COALESCE(SUM(CASE WHEN T2.Code = 'Resolved' THEN 1 ELSE 0 END), 0) AS ResolvedTickets,
            COALESCE(SUM(CASE WHEN T2.Code = 'ReOpened' THEN 1 ELSE 0 END), 0) AS ReOpenedTickets
        FROM tickets T1
        LEFT JOIN systemcodedetails T2 ON T1.StatusId = T2.Id;
    ";

            Database.ExecuteSqlRaw(createViewSql);
        }

    }
}
