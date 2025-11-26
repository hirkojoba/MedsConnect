using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedsConnect.Services;
using MedsConnect.Views;

namespace MedsConnect.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IMedicationNotificationService _notificationService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private DateTime dateOfBirth = DateTime.Now.AddYears(-25);

    [ObservableProperty]
    private string selectedRole = "Patient";

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public List<string> UserRoles { get; } = new() { "Patient", "Caregiver" };

    public RegisterViewModel(IAuthenticationService authService, IMedicationNotificationService notificationService)
    {
        _authService = authService;
        _notificationService = notificationService;
        Title = "Register";
    }

    [RelayCommand]
    async Task RegisterAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "All fields are required.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters long.";
                return;
            }

            var (success, message, user) = await _authService.RegisterAsync(
                Username, Email, Password, FirstName, LastName, DateOfBirth, SelectedRole);

            if (success)
            {
                // Request notification permissions
                await _notificationService.RequestPermissionsAsync();

                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            }
            else
            {
                ErrorMessage = message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
