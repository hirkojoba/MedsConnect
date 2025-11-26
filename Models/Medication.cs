using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedsConnect.Models;

public class Medication
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public string Dosage { get; set; } = string.Empty;

    public string Unit { get; set; } = "mg"; // mg, ml, tablets, etc.

    [Required]
    public string Frequency { get; set; } = string.Empty; // Daily, Twice daily, Weekly, etc.

    public List<TimeSpan> ScheduledTimes { get; set; } = new List<TimeSpan>();

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }

    public bool ReminderEnabled { get; set; } = true;

    public int ReminderMinutesBefore { get; set; } = 15;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public ICollection<MedicationLog> MedicationLogs { get; set; } = new List<MedicationLog>();
}
