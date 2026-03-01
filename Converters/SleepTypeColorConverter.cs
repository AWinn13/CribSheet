using CribSheet.Models;
using System.Globalization;

namespace CribSheet.Converters
{
  public class SleepTypeColorConverter : IValueConverter
  {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is SleepType sleepType)
        return sleepType == SleepType.Night
          ? Color.FromArgb("#1A237E")
          : Color.FromArgb("#42A5F5");

      return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
      => throw new NotImplementedException();
  }
}
