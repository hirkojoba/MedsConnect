using MedsConnect.Models;

namespace MedsConnect.Services;

public interface IMedicationService
{
    Task<List<Medication>> GetAllMedicationsAsync(int userId);
    Task<List<Medication>> GetActiveMedicationsAsync(int userId);
    Task<Medication?> GetMedicationByIdAsync(int medicationId);
    Task<(bool Success, string Message)> AddMedicationAsync(Medication medication);
    Task<(bool Success, string Message)> UpdateMedicationAsync(Medication medication);
    Task<(bool Success, string Message)> DeleteMedicationAsync(int medicationId);
    Task<List<Medication>> GetTodaysMedicationsAsync(int userId);
}
