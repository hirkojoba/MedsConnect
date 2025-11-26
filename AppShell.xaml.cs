using MedsConnect.Views;

namespace MedsConnect;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(AddEditMedicationPage), typeof(AddEditMedicationPage));
    }
}
