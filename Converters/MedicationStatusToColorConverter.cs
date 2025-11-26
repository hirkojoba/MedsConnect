using System.Globalization;
using MedsConnect.Models;

namespace MedsConnect.Converters;

public class MedicationStatusToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MedicationStatus status)
        {
            return status switch
            {
                MedicationStatus.Taken => Colors.Green,
                MedicationStatus.Missed => Colors.Red,
                MedicationStatus.Skipped => Colors.Gray,
                MedicationStatus.Pending => Colors.Orange,
                _ => Colors.Black
            };
        }
        return Colors.Black;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
