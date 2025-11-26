using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedsConnect.Models;

public class CaregiverRelationship
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int CaregiverId { get; set; }

    public string Relationship { get; set; } = string.Empty; // Family, Friend, Professional, etc.

    public bool IsApproved { get; set; } = false;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAt { get; set; }

    public bool CanViewMedications { get; set; } = true;

    public bool CanViewLogs { get; set; } = true;

    public bool CanReceiveAlerts { get; set; } = true;

    // Navigation properties
    [ForeignKey("PatientId")]
    public User Patient { get; set; } = null!;

    [ForeignKey("CaregiverId")]
    public User Caregiver { get; set; } = null!;
}
