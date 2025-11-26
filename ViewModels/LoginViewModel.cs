using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedsConnect.Services;
using MedsConnect.Views;

namespace MedsConnect.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IMedicationNotificationService _notificationService;

    [ObservableProperty]
    private string emailOrUsername = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(IAuthenticationService authService, IMedicationNotificationService notificationService)
    {
        _authService = authService;
        _notificationService = notificationService;
        Title = "Login";
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email/username and password.";
                return;
            }

            var (success, message, user) = await _authService.LoginAsync(EmailOrUsername, Password);

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
    async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
