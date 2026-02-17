using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CribSheet.Converters
{
  internal class OzToLbsOzConverter : IValueConverter
  {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is long totalOunces)
      {
        long lbs = totalOunces / 16;
        long oz = totalOunces % 16;
        return (string)$"{lbs} lbs {oz} oz";
      }
      return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      return string.Empty;
    }
  }
}
