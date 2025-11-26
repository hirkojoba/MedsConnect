using System.ComponentModel.DataAnnotations;

namespace MedsConnect.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string UserRole { get; set; } = "Patient"; // Patient or Caregiver

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<Medication> Medications { get; set; } = new List<Medication>();
    public ICollection<MedicationLog> MedicationLogs { get; set; } = new List<MedicationLog>();
    public ICollection<CaregiverRelationship> Caregivers { get; set; } = new List<CaregiverRelationship>();
    public ICollection<CaregiverRelationship> Patients { get; set; } = new List<CaregiverRelationship>();
}
