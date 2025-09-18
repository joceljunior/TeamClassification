using Microsoft.EntityFrameworkCore;
using TeamClassification.Models;

namespace TeamClassification.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<ScoreHistory> ScoreHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Team entity
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalScore).HasConversion(
                    v => v.Ticks,
                    v => new TimeSpan(v));
            });

            // Configure Participant entity
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalScore).HasConversion(
                    v => v.Ticks,
                    v => new TimeSpan(v));
                entity.Property(e => e.InitialTime).HasConversion(
                    v => v.Ticks,
                    v => new TimeSpan(v));
                
                entity.HasOne(e => e.Team)
                    .WithMany(e => e.Participants)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ScoreHistory entity
            modelBuilder.Entity<ScoreHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ScoreChange).HasConversion(
                    v => v.Ticks,
                    v => new TimeSpan(v));
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(e => e.Team)
                    .WithMany(e => e.ScoreHistories)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Participant)
                    .WithMany(e => e.ScoreHistories)
                    .HasForeignKey(e => e.ParticipantId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed data removido para evitar conflitos
        }
    }
}
