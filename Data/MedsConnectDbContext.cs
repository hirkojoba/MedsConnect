using Microsoft.EntityFrameworkCore;
using MedsConnect.Models;
using System.Text.Json;

namespace MedsConnect.Data;

public class MedsConnectDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<MedicationLog> MedicationLogs { get; set; }
    public DbSet<CaregiverRelationship> CaregiverRelationships { get; set; }

    public MedsConnectDbContext(DbContextOptions<MedsConnectDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Configure Medication entity
        modelBuilder.Entity<Medication>()
            .HasOne(m => m.User)
            .WithMany(u => u.Medications)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Convert List<TimeSpan> to JSON for SQLite storage
        modelBuilder.Entity<Medication>()
            .Property(m => m.ScheduledTimes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<TimeSpan>>(v, (JsonSerializerOptions?)null) ?? new List<TimeSpan>());

        // Configure MedicationLog entity
        modelBuilder.Entity<MedicationLog>()
            .HasOne(ml => ml.Medication)
            .WithMany(m => m.MedicationLogs)
            .HasForeignKey(ml => ml.MedicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MedicationLog>()
            .HasOne(ml => ml.User)
            .WithMany(u => u.MedicationLogs)
            .HasForeignKey(ml => ml.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicationLog>()
            .HasOne(ml => ml.MarkedByUser)
            .WithMany()
            .HasForeignKey(ml => ml.MarkedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CaregiverRelationship entity
        modelBuilder.Entity<CaregiverRelationship>()
            .HasOne(cr => cr.Patient)
            .WithMany(u => u.Caregivers)
            .HasForeignKey(cr => cr.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CaregiverRelationship>()
            .HasOne(cr => cr.Caregiver)
            .WithMany(u => u.Patients)
            .HasForeignKey(cr => cr.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
