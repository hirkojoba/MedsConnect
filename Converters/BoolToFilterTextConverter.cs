using System.Globalization;

namespace MedsConnect.Converters;

public class BoolToFilterTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool showActiveOnly && showActiveOnly ? "Show All" : "Show Active Only";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
