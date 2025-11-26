using Microsoft.EntityFrameworkCore;
using MedsConnect.Data;
using MedsConnect.Models;

namespace MedsConnect.Services;

public class CaregiverService : ICaregiverService
{
    private readonly MedsConnectDbContext _context;

    public CaregiverService(MedsConnectDbContext context)
    {
        _context = context;
    }

    public async Task<List<CaregiverRelationship>> GetMyCaregiversAsync(int patientId)
    {
        return await _context.CaregiverRelationships
            .Include(cr => cr.Caregiver)
            .Where(cr => cr.PatientId == patientId && cr.IsApproved)
            .ToListAsync();
    }

    public async Task<List<CaregiverRelationship>> GetMyPatientsAsync(int caregiverId)
    {
        return await _context.CaregiverRelationships
            .Include(cr => cr.Patient)
            .Where(cr => cr.CaregiverId == caregiverId && cr.IsApproved)
            .ToListAsync();
    }

    public async Task<List<CaregiverRelationship>> GetPendingRequestsAsync(int userId)
    {
        return await _context.CaregiverRelationships
            .Include(cr => cr.Patient)
            .Include(cr => cr.Caregiver)
            .Where(cr => (cr.PatientId == userId || cr.CaregiverId == userId) && !cr.IsApproved)
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> SendCaregiverRequestAsync(int patientId, string caregiverEmail, string relationship)
    {
        try
        {
            var caregiver = await _context.Users.FirstOrDefaultAsync(u => u.Email == caregiverEmail);
            if (caregiver == null)
            {
                return (false, "Caregiver with this email not found.");
            }

            if (caregiver.Id == patientId)
            {
                return (false, "You cannot add yourself as a caregiver.");
            }

            // Check if relationship already exists
            var existing = await _context.CaregiverRelationships
                .FirstOrDefaultAsync(cr => cr.PatientId == patientId && cr.CaregiverId == caregiver.Id);

            if (existing != null)
            {
                return (false, "Caregiver relationship already exists.");
            }

            var caregiverRelationship = new CaregiverRelationship
            {
                PatientId = patientId,
                CaregiverId = caregiver.Id,
                Relationship = relationship,
                RequestedAt = DateTime.UtcNow,
                IsApproved = false
            };

            _context.CaregiverRelationships.Add(caregiverRelationship);
            await _context.SaveChangesAsync();

            return (true, "Caregiver request sent successfully!");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to send request: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> ApproveCaregiverRequestAsync(int relationshipId)
    {
        try
        {
            var relationship = await _context.CaregiverRelationships.FindAsync(relationshipId);
            if (relationship == null)
            {
                return (false, "Request not found.");
            }

            relationship.IsApproved = true;
            relationship.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Caregiver request approved!");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to approve request: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> RejectCaregiverRequestAsync(int relationshipId)
    {
        try
        {
            var relationship = await _context.CaregiverRelationships.FindAsync(relationshipId);
            if (relationship == null)
            {
                return (false, "Request not found.");
            }

            _context.CaregiverRelationships.Remove(relationship);
            await _context.SaveChangesAsync();

            return (true, "Request rejected.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to reject request: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> RemoveCaregiverAsync(int relationshipId)
    {
        try
        {
            var relationship = await _context.CaregiverRelationships.FindAsync(relationshipId);
            if (relationship == null)
            {
                return (false, "Relationship not found.");
            }

            _context.CaregiverRelationships.Remove(relationship);
            await _context.SaveChangesAsync();

            return (true, "Caregiver removed successfully.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to remove caregiver: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> UpdatePermissionsAsync(int relationshipId, bool canViewMedications, bool canViewLogs, bool canReceiveAlerts)
    {
        try
        {
            var relationship = await _context.CaregiverRelationships.FindAsync(relationshipId);
            if (relationship == null)
            {
                return (false, "Relationship not found.");
            }

            relationship.CanViewMedications = canViewMedications;
            relationship.CanViewLogs = canViewLogs;
            relationship.CanReceiveAlerts = canReceiveAlerts;

            await _context.SaveChangesAsync();
            return (true, "Permissions updated successfully!");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to update permissions: {ex.Message}");
        }
    }
}
