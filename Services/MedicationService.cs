using Microsoft.EntityFrameworkCore;
using MedsConnect.Data;
using MedsConnect.Models;

namespace MedsConnect.Services;

public class MedicationService : IMedicationService
{
    private readonly MedsConnectDbContext _context;

    public MedicationService(MedsConnectDbContext context)
    {
        _context = context;
    }

    public async Task<List<Medication>> GetAllMedicationsAsync(int userId)
    {
        return await _context.Medications
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<List<Medication>> GetActiveMedicationsAsync(int userId)
    {
        return await _context.Medications
            .Where(m => m.UserId == userId && m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Medication?> GetMedicationByIdAsync(int medicationId)
    {
        return await _context.Medications
            .FirstOrDefaultAsync(m => m.Id == medicationId);
    }

    public async Task<(bool Success, string Message)> AddMedicationAsync(Medication medication)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(medication.Name))
            {
                return (false, "Medication name is required.");
            }

            medication.CreatedAt = DateTime.UtcNow;
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();

            return (true, "Medication added successfully!");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to add medication: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> UpdateMedicationAsync(Medication medication)
    {
        try
        {
            var existingMedication = await _context.Medications.FindAsync(medication.Id);
            if (existingMedication == null)
            {
                return (false, "Medication not found.");
            }

            existingMedication.Name = medication.Name;
            existingMedication.Description = medication.Description;
            existingMedication.Dosage = medication.Dosage;
            existingMedication.Unit = medication.Unit;
            existingMedication.Frequency = medication.Frequency;
            existingMedication.ScheduledTimes = medication.ScheduledTimes;
            existingMedication.StartDate = medication.StartDate;
            existingMedication.EndDate = medication.EndDate;
            existingMedication.IsActive = medication.IsActive;
            existingMedication.Notes = medication.Notes;
            existingMedication.ReminderEnabled = medication.ReminderEnabled;
            existingMedication.ReminderMinutesBefore = medication.ReminderMinutesBefore;
            existingMedication.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Medication updated successfully!");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to update medication: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> DeleteMedicationAsync(int medicationId)
    {
        try
        {
            var medication = await _context.Medications.FindAsync(medicationId);
            if (medication == null)
            {
                return (false, "Medication not found.");
            }

            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();

            return (true, "Medication deleted successfully!");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to delete medication: {ex.Message}");
        }
    }

    public async Task<List<Medication>> GetTodaysMedicationsAsync(int userId)
    {
        var today = DateTime.Today;
        return await _context.Medications
            .Where(m => m.UserId == userId &&
                       m.IsActive &&
                       m.StartDate <= today &&
                       (m.EndDate == null || m.EndDate >= today))
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}
