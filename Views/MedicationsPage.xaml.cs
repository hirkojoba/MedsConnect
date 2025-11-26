using MedsConnect.ViewModels;

namespace MedsConnect.Views;

public partial class MedicationsPage : ContentPage
{
    private readonly MedicationsViewModel _viewModel;

    public MedicationsPage(MedicationsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
