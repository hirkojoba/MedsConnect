using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedsConnect.Models;
using MedsConnect.Services;
using System.Collections.ObjectModel;

namespace MedsConnect.ViewModels;

[QueryProperty(nameof(MedicationId), "MedicationId")]
public partial class AddEditMedicationViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IMedicationService _medicationService;
    private readonly IMedicationNotificationService _notificationService;
    private readonly IMedicationLogService _logService;

    [ObservableProperty]
    private int medicationId;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string dosage = string.Empty;

    [ObservableProperty]
    private string unit = "mg";

    [ObservableProperty]
    private string frequency = "Daily";

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime? endDate;

    [ObservableProperty]
    private bool hasEndDate;

    [ObservableProperty]
    private string notes = string.Empty;

    [ObservableProperty]
    private bool reminderEnabled = true;

    [ObservableProperty]
    private int reminderMinutesBefore = 15;

    [ObservableProperty]
    private ObservableCollection<TimeSpan> scheduledTimes = new();

    [ObservableProperty]
    private TimeSpan selectedTime = new TimeSpan(9, 0, 0);

    [ObservableProperty]
    private bool isActive = true;

    [ObservableProperty]
    private string reminderSummary = string.Empty;

    public List<string> Units { get; } = new() { "mg", "ml", "tablets", "capsules", "drops", "units" };
    public List<string> Frequencies { get; } = new() { "Daily", "Twice Daily", "Three Times Daily", "Four Times Daily", "Weekly", "As Needed" };
    public List<int> ReminderOptions { get; } = new() { 0, 5, 10, 15, 30, 60 };

    public AddEditMedicationViewModel(
        IAuthenticationService authService,
        IMedicationService medicationService,
        IMedicationNotificationService notificationService,
        IMedicationLogService logService)
    {
        _authService = authService;
        _medicationService = medicationService;
        _notificationService = notificationService;
        _logService = logService;
        Title = "Add Medication";
    }

    partial void OnMedicationIdChanged(int value)
    {
        if (value > 0)
        {
            Title = "Edit Medication";
            Task.Run(LoadMedicationAsync);
        }
    }

    async Task LoadMedicationAsync()
    {
        var medication = await _medicationService.GetMedicationByIdAsync(MedicationId);
        if (medication != null)
        {
            Name = medication.Name;
            Description = medication.Description;
            Dosage = medication.Dosage;
            Unit = medication.Unit;
            Frequency = medication.Frequency;
            StartDate = medication.StartDate;
            EndDate = medication.EndDate;
            HasEndDate = medication.EndDate.HasValue;
            Notes = medication.Notes ?? string.Empty;
            ReminderEnabled = medication.ReminderEnabled;
            ReminderMinutesBefore = medication.ReminderMinutesBefore;
            IsActive = medication.IsActive;
            ScheduledTimes = new ObservableCollection<TimeSpan>(medication.ScheduledTimes);
            UpdateReminderSummary();
        }
    }

    [RelayCommand]
    void AddTime()
    {
        if (!ScheduledTimes.Contains(SelectedTime))
        {
            ScheduledTimes.Add(SelectedTime);
            ScheduledTimes = new ObservableCollection<TimeSpan>(ScheduledTimes.OrderBy(t => t));
            UpdateReminderSummary();
        }
    }

    [RelayCommand]
    void RemoveTime(TimeSpan time)
    {
        ScheduledTimes.Remove(time);
        UpdateReminderSummary();
    }

    partial void OnReminderEnabledChanged(bool value)
    {
        UpdateReminderSummary();
    }

    partial void OnReminderMinutesBeforeChanged(int value)
    {
        UpdateReminderSummary();
    }

    private void UpdateReminderSummary()
    {
        if (!ReminderEnabled || ScheduledTimes.Count == 0)
        {
            ReminderSummary = "No reminders set";
            return;
        }

        var reminderText = ReminderMinutesBefore == 0
            ? "at the scheduled time"
            : $"{ReminderMinutesBefore} minutes before";

        var times = string.Join(", ", ScheduledTimes.OrderBy(t => t).Select(t =>
            DateTime.Today.Add(t).ToString("h:mm tt")));

        ReminderSummary = $"You will receive notifications {reminderText}:\n{times}";
    }

    [RelayCommand]
    async Task SaveAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Error", "Medication name is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Dosage))
            {
                await Shell.Current.DisplayAlert("Error", "Dosage is required.", "OK");
                return;
            }

            if (ScheduledTimes.Count == 0)
            {
                await Shell.Current.DisplayAlert("Error", "Please add at least one scheduled time.", "OK");
                return;
            }

            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var medication = new Medication
            {
                Id = MedicationId,
                UserId = user.Id,
                Name = Name,
                Description = Description,
                Dosage = Dosage,
                Unit = Unit,
                Frequency = Frequency,
                StartDate = StartDate,
                EndDate = HasEndDate ? EndDate : null,
                Notes = Notes,
                ReminderEnabled = ReminderEnabled,
                ReminderMinutesBefore = ReminderMinutesBefore,
                IsActive = IsActive,
                ScheduledTimes = ScheduledTimes.ToList()
            };

            (bool Success, string Message) result;

            if (MedicationId > 0)
            {
                result = await _medicationService.UpdateMedicationAsync(medication);
                if (result.Success && ReminderEnabled)
                {
                    await _notificationService.RescheduleMedicationRemindersAsync(medication);
                }
            }
            else
            {
                result = await _medicationService.AddMedicationAsync(medication);
                if (result.Success && ReminderEnabled)
                {
                    // Get the newly created medication with its ID
                    var user2 = await _authService.GetCurrentUserAsync();
                    if (user2 != null)
                    {
                        var meds = await _medicationService.GetActiveMedicationsAsync(user2.Id);
                        var newMed = meds.FirstOrDefault(m => m.Name == Name);
                        if (newMed != null)
                        {
                            await _notificationService.ScheduleMedicationRemindersAsync(newMed);
                        }
                    }
                }
            }

            if (result.Success)
            {
                // Generate logs for today so the medication appears on the dashboard immediately
                await _logService.GenerateLogsForTodayAsync(user.Id);

                await Shell.Current.DisplayAlert("Success", result.Message, "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save medication: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
