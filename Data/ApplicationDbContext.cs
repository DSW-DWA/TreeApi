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

        public DbSet<Tree> Trees { get; set; }

        public DbSet<Node> Nodes { get; set; }

        public DbSet<ExceptionJournal> ExceptionJournals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tree>()
                .HasMany(t => t.Nodes)
                .WithOne(n => n.Tree)
                .HasForeignKey(n => n.TreeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Node>()
                .HasIndex(n => new { n.TreeId, n.Name })
                .IsUnique();

            modelBuilder.Entity<ExceptionJournal>()
                .Property(j => j.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}
