using MedsConnect.ViewModels;

namespace MedsConnect.Views;

public partial class CaregiversPage : ContentPage
{
    private readonly CaregiversViewModel _viewModel;

    public CaregiversPage(CaregiversViewModel viewModel)
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
