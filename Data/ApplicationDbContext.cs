using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TreeApi.Models;

namespace TreeApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for Trees
        public DbSet<Tree> Trees { get; set; }

        // DbSet for Nodes
        public DbSet<Node> Nodes { get; set; }

        // DbSet for Exception Journals
        public DbSet<ExceptionJournal> ExceptionJournals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Tree model
            modelBuilder.Entity<Tree>()
                .HasMany(t => t.Nodes)
                .WithOne(n => n.Tree)
                .HasForeignKey(n => n.TreeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure the Node name is unique within a tree's hierarchy
            modelBuilder.Entity<Node>()
                .HasIndex(n => new { n.TreeId, n.Name })
                .IsUnique();

            // Configure ExceptionJournal model
            modelBuilder.Entity<ExceptionJournal>()
                .Property(j => j.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Additional configurations can be done here as needed
        }
    }
}
