using MedsConnect.Models;

namespace MedsConnect.Services;

public interface IAuthenticationService
{
    Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string email, string password, string firstName, string lastName, DateTime dateOfBirth, string userRole = "Patient");
    Task<(bool Success, string Message, User? User)> LoginAsync(string emailOrUsername, string password);
    Task LogoutAsync();
    Task<User?> GetCurrentUserAsync();
    bool IsLoggedIn { get; }
}
