using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedsConnect.Models;
using MedsConnect.Services;
using System.Collections.ObjectModel;

namespace MedsConnect.ViewModels;

public partial class HistoryViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IMedicationLogService _logService;

    [ObservableProperty]
    private ObservableCollection<MedicationLog> logs = new();

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private Dictionary<DateTime, int> adherenceStats = new();

    public HistoryViewModel(
        IAuthenticationService authService,
        IMedicationLogService logService)
    {
        _authService = authService;
        _logService = logService;
        Title = "History";
    }

    public async Task InitializeAsync()
    {
        await LoadHistoryAsync();
    }

    [RelayCommand]
    async Task LoadHistoryAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var logsList = await _logService.GetLogsForDateAsync(user.Id, SelectedDate);
            Logs = new ObservableCollection<MedicationLog>(logsList);

            // Load adherence stats for the past 30 days
            AdherenceStats = await _logService.GetAdherenceStatsAsync(user.Id, 30);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load history: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task DateChangedAsync()
    {
        await LoadHistoryAsync();
    }

    [RelayCommand]
    async Task PreviousDayAsync()
    {
        SelectedDate = SelectedDate.AddDays(-1);
        await LoadHistoryAsync();
    }

    [RelayCommand]
    async Task NextDayAsync()
    {
        if (SelectedDate < DateTime.Today)
        {
            SelectedDate = SelectedDate.AddDays(1);
            await LoadHistoryAsync();
        }
    }

    [RelayCommand]
    async Task TodayAsync()
    {
        SelectedDate = DateTime.Today;
        await LoadHistoryAsync();
    }
}
