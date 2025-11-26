using Microsoft.EntityFrameworkCore;
using MedsConnect.Data;
using MedsConnect.Models;

namespace MedsConnect.Services;

public class MedicationLogService : IMedicationLogService
{
    private readonly MedsConnectDbContext _context;
    private readonly IMedicationService _medicationService;

    public MedicationLogService(MedsConnectDbContext context, IMedicationService medicationService)
    {
        _context = context;
        _medicationService = medicationService;
    }

    public async Task<List<MedicationLog>> GetLogsForDateAsync(int userId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.MedicationLogs
            .Include(ml => ml.Medication)
            .Where(ml => ml.UserId == userId &&
                        ml.ScheduledDateTime >= startOfDay &&
                        ml.ScheduledDateTime < endOfDay)
            .OrderBy(ml => ml.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<List<MedicationLog>> GetLogsForMedicationAsync(int medicationId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.MedicationLogs
            .Include(ml => ml.Medication)
            .Where(ml => ml.MedicationId == medicationId);

        if (startDate.HasValue)
        {
            query = query.Where(ml => ml.ScheduledDateTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(ml => ml.ScheduledDateTime <= endDate.Value);
        }

        return await query
            .OrderByDescending(ml => ml.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<MedicationLog?> GetLogByIdAsync(int logId)
    {
        return await _context.MedicationLogs
            .Include(ml => ml.Medication)
            .FirstOrDefaultAsync(ml => ml.Id == logId);
    }

    public async Task<(bool Success, string Message)> MarkAsTakenAsync(int logId, DateTime? takenTime = null, string? notes = null)
    {
        try
        {
            var log = await _context.MedicationLogs.FindAsync(logId);
            if (log == null)
            {
                return (false, "Log entry not found.");
            }

            log.Status = MedicationStatus.Taken;
            log.TakenDateTime = takenTime ?? DateTime.Now;
            log.Notes = notes;

            await _context.SaveChangesAsync();
            return (true, "Marked as taken!");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to update log: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> MarkAsMissedAsync(int logId, string? notes = null)
    {
        try
        {
            var log = await _context.MedicationLogs.FindAsync(logId);
            if (log == null)
            {
                return (false, "Log entry not found.");
            }

            log.Status = MedicationStatus.Missed;
            log.Notes = notes;

            await _context.SaveChangesAsync();
            return (true, "Marked as missed.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to update log: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> MarkAsSkippedAsync(int logId, string? notes = null)
    {
        try
        {
            var log = await _context.MedicationLogs.FindAsync(logId);
            if (log == null)
            {
                return (false, "Log entry not found.");
            }

            log.Status = MedicationStatus.Skipped;
            log.Notes = notes;

            await _context.SaveChangesAsync();
            return (true, "Marked as skipped.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to update log: {ex.Message}");
        }
    }

    public async Task GenerateLogsForTodayAsync(int userId)
    {
        var today = DateTime.Today;
        var medications = await _medicationService.GetTodaysMedicationsAsync(userId);

        foreach (var medication in medications)
        {
            foreach (var time in medication.ScheduledTimes)
            {
                var scheduledDateTime = today.Add(time);

                // Check if log already exists
                var existingLog = await _context.MedicationLogs
                    .FirstOrDefaultAsync(ml =>
                        ml.MedicationId == medication.Id &&
                        ml.ScheduledDateTime == scheduledDateTime);

                if (existingLog == null)
                {
                    var log = new MedicationLog
                    {
                        MedicationId = medication.Id,
                        UserId = userId,
                        ScheduledDateTime = scheduledDateTime,
                        Status = MedicationStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.MedicationLogs.Add(log);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Dictionary<DateTime, int>> GetAdherenceStatsAsync(int userId, int days = 30)
    {
        var startDate = DateTime.Today.AddDays(-days);
        var logs = await _context.MedicationLogs
            .Where(ml => ml.UserId == userId && ml.ScheduledDateTime >= startDate)
            .ToListAsync();

        var stats = logs
            .GroupBy(ml => ml.ScheduledDateTime.Date)
            .ToDictionary(
                g => g.Key,
                g => (int)Math.Round((double)g.Count(l => l.Status == MedicationStatus.Taken) / g.Count() * 100)
            );

        return stats;
    }
}
