using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedsConnect.Models;
using MedsConnect.Services;
using System.Collections.ObjectModel;

namespace MedsConnect.ViewModels;

public partial class CaregiversViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly ICaregiverService _caregiverService;

    [ObservableProperty]
    private ObservableCollection<CaregiverRelationship> myCaregivers = new();

    [ObservableProperty]
    private ObservableCollection<CaregiverRelationship> myPatients = new();

    [ObservableProperty]
    private ObservableCollection<CaregiverRelationship> pendingRequests = new();

    [ObservableProperty]
    private string caregiverEmail = string.Empty;

    [ObservableProperty]
    private string relationship = "Family";

    [ObservableProperty]
    private bool showAddCaregiverForm;

    public List<string> RelationshipTypes { get; } = new() { "Family", "Friend", "Professional Caregiver", "Other" };

    public CaregiversViewModel(
        IAuthenticationService authService,
        ICaregiverService caregiverService)
    {
        _authService = authService;
        _caregiverService = caregiverService;
        Title = "Caregivers";
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
            if (user == null) return;

            var caregivers = await _caregiverService.GetMyCaregiversAsync(user.Id);
            MyCaregivers = new ObservableCollection<CaregiverRelationship>(caregivers);

            var patients = await _caregiverService.GetMyPatientsAsync(user.Id);
            MyPatients = new ObservableCollection<CaregiverRelationship>(patients);

            var pending = await _caregiverService.GetPendingRequestsAsync(user.Id);
            PendingRequests = new ObservableCollection<CaregiverRelationship>(pending);
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
    void ToggleAddForm()
    {
        ShowAddCaregiverForm = !ShowAddCaregiverForm;
    }

    [RelayCommand]
    async Task SendRequestAsync()
    {
        if (string.IsNullOrWhiteSpace(CaregiverEmail))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter caregiver's email.", "OK");
            return;
        }

        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var result = await _caregiverService.SendCaregiverRequestAsync(user.Id, CaregiverEmail, Relationship);

            if (result.Success)
            {
                await Shell.Current.DisplayAlert("Success", result.Message, "OK");
                CaregiverEmail = string.Empty;
                ShowAddCaregiverForm = false;
                await LoadDataAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to send request: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task ApproveRequestAsync(CaregiverRelationship request)
    {
        try
        {
            var result = await _caregiverService.ApproveCaregiverRequestAsync(request.Id);

            if (result.Success)
            {
                await Shell.Current.DisplayAlert("Success", result.Message, "OK");
                await LoadDataAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task RejectRequestAsync(CaregiverRelationship request)
    {
        try
        {
            var result = await _caregiverService.RejectCaregiverRequestAsync(request.Id);

            if (result.Success)
            {
                await LoadDataAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task RemoveCaregiverAsync(CaregiverRelationship caregiver)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Remove Caregiver",
            "Are you sure you want to remove this caregiver?",
            "Yes", "No");

        if (confirm)
        {
            var result = await _caregiverService.RemoveCaregiverAsync(caregiver.Id);
            if (result.Success)
            {
                await LoadDataAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }
    }
}
