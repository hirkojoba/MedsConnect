using Microsoft.EntityFrameworkCore;
using MedsConnect.Data;
using MedsConnect.Helpers;
using MedsConnect.Models;

namespace MedsConnect.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly MedsConnectDbContext _context;
    private User? _currentUser;

    public AuthenticationService(MedsConnectDbContext context)
    {
        _context = context;
    }

    public bool IsLoggedIn => _currentUser != null;

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(
        string username,
        string email,
        string password,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string userRole = "Patient")
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "All fields are required.", null);
            }

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email || u.Username == username);

            if (existingUser != null)
            {
                return (false, "Username or email already exists.", null);
            }

            // Create new user
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = PasswordHelper.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                UserRole = userRole,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _currentUser = user;

            // Save user ID to preferences
            await SecureStorage.SetAsync("user_id", user.Id.ToString());

            return (true, "Registration successful!", user);
        }
        catch (Exception ex)
        {
            return (false, $"Registration failed: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(string emailOrUsername, string password)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

            if (user == null)
            {
                return (false, "User not found.", null);
            }

            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                return (false, "Invalid password.", null);
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _currentUser = user;

            // Save user ID to preferences
            await SecureStorage.SetAsync("user_id", user.Id.ToString());

            return (true, "Login successful!", user);
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}", null);
        }
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        SecureStorage.Remove("user_id");
        await Task.CompletedTask;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        var userIdStr = await SecureStorage.GetAsync("user_id");
        if (int.TryParse(userIdStr, out int userId))
        {
            _currentUser = await _context.Users.FindAsync(userId);
        }

        return _currentUser;
    }
}
