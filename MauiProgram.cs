using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedsConnect.Data;
using MedsConnect.Services;
using MedsConnect.ViewModels;
using MedsConnect.Views;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;

namespace MedsConnect;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                // Use system fonts if custom fonts are not available
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Database
        try
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "medsconnect.db3");
            builder.Services.AddDbContext<MedsConnectDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database setup error: {ex.Message}");
        }

        // Services
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<IMedicationService, MedicationService>();
        builder.Services.AddSingleton<IMedicationLogService, MedicationLogService>();
        builder.Services.AddSingleton<ICaregiverService, CaregiverService>();
        builder.Services.AddSingleton<IMedicationNotificationService, NotificationService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<MedicationsViewModel>();
        builder.Services.AddTransient<AddEditMedicationViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<CaregiversViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<MedicationsPage>();
        builder.Services.AddTransient<AddEditMedicationPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<CaregiversPage>();

        var app = builder.Build();

        // Initialize database
        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MedsConnectDbContext>();
                db.Database.EnsureCreated();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
        }

        return app;
    }
}
