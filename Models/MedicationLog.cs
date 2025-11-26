using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedsConnect.Models;

public class MedicationLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MedicationId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime ScheduledDateTime { get; set; }

    public DateTime? TakenDateTime { get; set; }

    [Required]
    public MedicationStatus Status { get; set; } = MedicationStatus.Pending;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Track who marked the medication (for caregiver scenario)
    public int? MarkedByUserId { get; set; }

    public DateTime? MarkedDateTime { get; set; }

    // Navigation properties
    [ForeignKey("MedicationId")]
    public Medication Medication { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("MarkedByUserId")]
    public User? MarkedByUser { get; set; }
}

public enum MedicationStatus
{
    Pending,
    Taken,
    Missed,
    Skipped
}
