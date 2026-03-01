using CribSheet.Models;
using System.Globalization;

namespace CribSheet.Converters
{
  public class SleepBlockBoundsConverter : IValueConverter
  {
    private const double MinutesPerDay = 1440.0;
    private const double MinBlockWidth = 0.005;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is not SleepRecord record)
        return new Rect(0, 0, 0, 0);

      double startMinutes = record.StartTime.TimeOfDay.TotalMinutes;
      double x = startMinutes / MinutesPerDay;

      double endMinutes;
      if (record.EndTime.HasValue)
      {
        // Clip to the end of the start day if sleep crosses midnight
        var dayEnd = record.StartTime.Date.AddDays(1);
        var effectiveEnd = record.EndTime.Value > dayEnd ? dayEnd : record.EndTime.Value;
        endMinutes = (effectiveEnd - record.StartTime.Date).TotalMinutes;
      }
      else
      {
        // Ongoing sleep: show up to current time, capped at end of day
        var now = DateTime.Now;
        var dayEnd = record.StartTime.Date.AddDays(1);
        var effectiveEnd = now < dayEnd ? now : dayEnd;
        endMinutes = (effectiveEnd - record.StartTime.Date).TotalMinutes;
      }

      double widthRatio = (endMinutes - startMinutes) / MinutesPerDay;
      widthRatio = Math.Max(widthRatio, MinBlockWidth);
      widthRatio = Math.Min(widthRatio, 1.0 - x);

      return new Rect(x, 0, widthRatio, 1.0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
      => throw new NotImplementedException();
  }
}
