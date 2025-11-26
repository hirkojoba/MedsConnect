using System.Globalization;
using MedsConnect.Models;

namespace MedsConnect.Converters;

public class MedicationStatusToPendingConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is MedicationStatus status && status == MedicationStatus.Pending;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
