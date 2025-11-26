using MedsConnect.Models;

namespace MedsConnect.Services;

public interface ICaregiverService
{
    Task<List<CaregiverRelationship>> GetMyCaregiversAsync(int patientId);
    Task<List<CaregiverRelationship>> GetMyPatientsAsync(int caregiverId);
    Task<List<CaregiverRelationship>> GetPendingRequestsAsync(int userId);
    Task<(bool Success, string Message)> SendCaregiverRequestAsync(int patientId, string caregiverEmail, string relationship);
    Task<(bool Success, string Message)> ApproveCaregiverRequestAsync(int relationshipId);
    Task<(bool Success, string Message)> RejectCaregiverRequestAsync(int relationshipId);
    Task<(bool Success, string Message)> RemoveCaregiverAsync(int relationshipId);
    Task<(bool Success, string Message)> UpdatePermissionsAsync(int relationshipId, bool canViewMedications, bool canViewLogs, bool canReceiveAlerts);
}
