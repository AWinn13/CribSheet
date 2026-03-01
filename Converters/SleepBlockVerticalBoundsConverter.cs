using CribSheet.Models;
using System.Globalization;

namespace CribSheet.Converters
{
  /// <summary>
  /// Converts a SleepRecord into AbsoluteLayout bounds for a vertical 24-hour timeline.
  /// X and Width are proportional (0–1). Y and Height are absolute pixels (60px per hour).
  /// Use LayoutFlags="XProportional,WidthProportional" on the child element.
  /// </summary>
  public class SleepBlockVerticalBoundsConverter : IValueConverter
  {
    public const double PixelsPerHour = 60.0;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is not SleepRecord record)
        return new Rect(0, 0, 0, 0);

      double y = record.StartTime.TimeOfDay.TotalHours * PixelsPerHour;

      double height;
      if (record.EndTime.HasValue)
      {
        // Clip to end of the start day if sleep crosses midnight
        var dayEnd = record.StartTime.Date.AddDays(1);
        var effectiveEnd = record.EndTime.Value > dayEnd ? dayEnd : record.EndTime.Value;
        height = Math.Max((effectiveEnd - record.StartTime).TotalHours * PixelsPerHour, 8.0);
      }
      else
      {
        // Ongoing sleep: show up to now, capped at end of day
        var now = DateTime.Now;
        var dayEnd = record.StartTime.Date.AddDays(1);
        var effectiveEnd = now < dayEnd ? now : dayEnd;
        height = Math.Max((effectiveEnd - record.StartTime).TotalHours * PixelsPerHour, 8.0);
      }

      // X and Width are proportional; Y and Height are absolute pixels
      return new Rect(0.03, y, 0.94, height);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
      => throw new NotImplementedException();
  }
}
