using Microsoft.EntityFrameworkCore;
using Ticket_System.Models;
using Ticket_System.Models.Enums;

namespace Ticket_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        //public DbSet<Team> Teams { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ticket>().Property(t => t.Priority).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(t => t.Status).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(t => t.Category).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(t => t.Team).HasConversion<string>();
            modelBuilder.Entity<User>().Property(u => u.Team).HasConversion<string>();
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Requester)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.RequesterId)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.ClaimedTickets)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull); 
        }
    }
}
