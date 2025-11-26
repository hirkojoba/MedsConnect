using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedsConnect.Models;
using MedsConnect.Services;
using MedsConnect.Views;
using System.Collections.ObjectModel;

namespace MedsConnect.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IMedicationService _medicationService;
    private readonly IMedicationLogService _logService;

    [ObservableProperty]
    private string welcomeMessage = "Welcome!";

    [ObservableProperty]
    private ObservableCollection<MedicationLog> todaysLogs = new();

    [ObservableProperty]
    private int totalMedications;

    [ObservableProperty]
    private int takenCount;

    [ObservableProperty]
    private int pendingCount;

    [ObservableProperty]
    private int missedCount;

    [ObservableProperty]
    private double adherencePercentage;

    public DashboardViewModel(
        IAuthenticationService authService,
        IMedicationService medicationService,
        IMedicationLogService logService)
    {
        _authService = authService;
        _medicationService = medicationService;
        _logService = logService;
        Title = "Dashboard";
    }

    public async Task InitializeAsync()
    {
        await LoadDataAsync();
    }

    [RelayCommand]
    async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                return;
            }

            WelcomeMessage = $"Welcome, {user.FirstName}!";

            // Generate logs for today if not already done
            await _logService.GenerateLogsForTodayAsync(user.Id);

            // Get today's logs
            var logs = await _logService.GetLogsForDateAsync(user.Id, DateTime.Today);
            TodaysLogs = new ObservableCollection<MedicationLog>(logs);

            // Calculate statistics
            TotalMedications = logs.Count;
            TakenCount = logs.Count(l => l.Status == MedicationStatus.Taken);
            PendingCount = logs.Count(l => l.Status == MedicationStatus.Pending);
            MissedCount = logs.Count(l => l.Status == MedicationStatus.Missed);

            if (TotalMedications > 0)
            {
                AdherencePercentage = Math.Round((double)TakenCount / TotalMedications * 100, 1);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task MarkAsTakenAsync(MedicationLog log)
    {
        try
        {
            var result = await _logService.MarkAsTakenAsync(log.Id);
            if (result.Success)
            {
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task MarkAsMissedAsync(MedicationLog log)
    {
        try
        {
            var result = await _logService.MarkAsMissedAsync(log.Id);
            if (result.Success)
            {
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task GoToMedicationsAsync()
    {
        await Shell.Current.GoToAsync($"///{nameof(MedicationsPage)}");
    }

    [RelayCommand]
    async Task GoToHistoryAsync()
    {
        await Shell.Current.GoToAsync($"///{nameof(HistoryPage)}");
    }

    [RelayCommand]
    async Task GoToCaregiversAsync()
    {
        await Shell.Current.GoToAsync($"///{nameof(CaregiversPage)}");
    }

    [RelayCommand]
    async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirm)
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
