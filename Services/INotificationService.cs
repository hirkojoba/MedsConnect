using MedsConnect.Models;

namespace MedsConnect.Services;

public interface IMedicationNotificationService
{
    Task ScheduleMedicationRemindersAsync(Medication medication);
    Task CancelMedicationRemindersAsync(int medicationId);
    Task RescheduleMedicationRemindersAsync(Medication medication);
    Task<bool> RequestPermissionsAsync();
}
