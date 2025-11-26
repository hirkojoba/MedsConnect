using MedsConnect.Models;

#if ANDROID
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
#endif

namespace MedsConnect.Services;

public class NotificationService : IMedicationNotificationService
{
    public async Task<bool> RequestPermissionsAsync()
    {
        try
        {
#if ANDROID
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            return await LocalNotificationCenter.Current.AreNotificationsEnabled();
#else
            return await Task.FromResult(false);
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error requesting notification permissions: {ex.Message}");
            return false;
        }
    }

    public async Task ScheduleMedicationRemindersAsync(Medication medication)
    {
        if (!medication.ReminderEnabled || medication.ScheduledTimes.Count == 0)
            return;

        try
        {
#if ANDROID
            foreach (var time in medication.ScheduledTimes)
            {
                var notificationId = GenerateNotificationId(medication.Id, time);
                var notifyTime = DateTime.Today.Add(time).AddMinutes(-medication.ReminderMinutesBefore);

                // If the notification time has passed today, schedule for tomorrow
                if (notifyTime < DateTime.Now)
                {
                    notifyTime = notifyTime.AddDays(1);
                }

                var request = new NotificationRequest
                {
                    NotificationId = notificationId,
                    Title = "Medication Reminder",
                    Description = $"Time to take {medication.Name} ({medication.Dosage}{medication.Unit})",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime,
                        RepeatType = NotificationRepeat.Daily
                    },
                    Android = new AndroidOptions
                    {
                        Priority = AndroidPriority.High,
                        VisibilityType = AndroidVisibilityType.Public,
                        AutoCancel = true
                    }
                };

                await LocalNotificationCenter.Current.Show(request);
            }
#else
            await Task.CompletedTask;
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error scheduling notification: {ex.Message}");
        }
    }

    public async Task CancelMedicationRemindersAsync(int medicationId)
    {
        try
        {
#if ANDROID
            // Cancel all notifications for this medication
            // We need to generate all possible notification IDs
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += 15)
                {
                    var time = new TimeSpan(hour, minute, 0);
                    var notificationId = GenerateNotificationId(medicationId, time);
                    LocalNotificationCenter.Current.Cancel(notificationId);
                }
            }
            await Task.CompletedTask;
#else
            await Task.CompletedTask;
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error canceling notification: {ex.Message}");
        }
    }

    public async Task RescheduleMedicationRemindersAsync(Medication medication)
    {
        await CancelMedicationRemindersAsync(medication.Id);
        await ScheduleMedicationRemindersAsync(medication);
    }

    private int GenerateNotificationId(int medicationId, TimeSpan time)
    {
        // Generate a unique notification ID based on medication ID and time
        return (medicationId * 10000) + (time.Hours * 100) + time.Minutes;
    }
}
