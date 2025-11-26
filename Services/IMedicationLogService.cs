using MedsConnect.Models;

namespace MedsConnect.Services;

public interface IMedicationLogService
{
    Task<List<MedicationLog>> GetLogsForDateAsync(int userId, DateTime date);
    Task<List<MedicationLog>> GetLogsForMedicationAsync(int medicationId, DateTime? startDate = null, DateTime? endDate = null);
    Task<MedicationLog?> GetLogByIdAsync(int logId);
    Task<(bool Success, string Message)> MarkAsTakenAsync(int logId, DateTime? takenTime = null, string? notes = null);
    Task<(bool Success, string Message)> MarkAsMissedAsync(int logId, string? notes = null);
    Task<(bool Success, string Message)> MarkAsSkippedAsync(int logId, string? notes = null);
    Task GenerateLogsForTodayAsync(int userId);
    Task<Dictionary<DateTime, int>> GetAdherenceStatsAsync(int userId, int days = 30);
}
