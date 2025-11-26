using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedsConnect.Models;
using MedsConnect.Services;
using MedsConnect.Views;
using System.Collections.ObjectModel;

namespace MedsConnect.ViewModels;

public partial class MedicationsViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly IMedicationService _medicationService;

    [ObservableProperty]
    private ObservableCollection<Medication> medications = new();

    [ObservableProperty]
    private bool showActiveOnly = true;

    public MedicationsViewModel(
        IAuthenticationService authService,
        IMedicationService medicationService)
    {
        _authService = authService;
        _medicationService = medicationService;
        Title = "Medications";
    }

    public async Task InitializeAsync()
    {
        await LoadMedicationsAsync();
    }

    [RelayCommand]
    async Task LoadMedicationsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            List<Medication> meds;
            if (ShowActiveOnly)
            {
                meds = await _medicationService.GetActiveMedicationsAsync(user.Id);
            }
            else
            {
                meds = await _medicationService.GetAllMedicationsAsync(user.Id);
            }

            Medications = new ObservableCollection<Medication>(meds);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load medications: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddMedicationAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddEditMedicationPage));
    }

    [RelayCommand]
    async Task EditMedicationAsync(Medication medication)
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditMedicationPage)}?MedicationId={medication.Id}");
    }

    [RelayCommand]
    async Task DeleteMedicationAsync(Medication medication)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Delete Medication",
            $"Are you sure you want to delete {medication.Name}?",
            "Yes", "No");

        if (confirm)
        {
            var result = await _medicationService.DeleteMedicationAsync(medication.Id);
            if (result.Success)
            {
                await LoadMedicationsAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }
    }

    [RelayCommand]
    async Task ToggleFilterAsync()
    {
        ShowActiveOnly = !ShowActiveOnly;
        await LoadMedicationsAsync();
    }
}
