using System.Globalization;

namespace MedsConnect.Converters;

public class BoolToAddCaregiverTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool showForm && showForm ? "Cancel" : "+ Add Caregiver";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
