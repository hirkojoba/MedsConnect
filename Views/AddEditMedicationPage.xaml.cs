using MedsConnect.ViewModels;

namespace MedsConnect.Views;

public partial class AddEditMedicationPage : ContentPage
{
    public AddEditMedicationPage(AddEditMedicationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
