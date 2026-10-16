
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();
        public DbSet<RaceDay.API.Models.Route> Routes
           => Set<RaceDay.API.Models.Route>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Event>().ToTable("Events");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Enrolment>().ToTable("Enrolments");
            modelBuilder.Entity<Result>().ToTable("Results");
            modelBuilder.Entity<RaceDay.API.Models.Route>()
                .ToTable("Routes");

            modelBuilder.Entity<Category>()
                .HasAlternateKey(c => new { c.EventID, c.CategoryID });

            modelBuilder.Entity<Enrolment>()
                .HasOne<Category>()
                .WithMany()
                .HasForeignKey(e => new { e.EventID, e.CategoryID })
                .HasPrincipalKey(c => new { c.EventID, c.CategoryID });

            modelBuilder.Entity<Event>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Enrolment>()
                .Property(e => e.EnrolmentDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Enrolment>()
                .Property(e => e.Status)
                .HasDefaultValue("Confirmed");

            modelBuilder.Entity<Result>()
                .Property(r => r.RecordedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Result>()
                .Property(r => r.ResultStatus)
                .HasDefaultValue("Recorded");
        }
    }
}
