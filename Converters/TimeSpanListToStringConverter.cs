using System.Globalization;

namespace MedsConnect.Converters;

public class TimeSpanListToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is List<TimeSpan> times && times.Count > 0)
        {
            var timeStrings = times.Select(t =>
            {
                var dateTime = DateTime.Today.Add(t);
                return dateTime.ToString("h:mm tt");
            });
            return "Times: " + string.Join(", ", timeStrings);
        }
        return "No times scheduled";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
