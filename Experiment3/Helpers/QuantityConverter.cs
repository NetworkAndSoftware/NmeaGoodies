using System;
using System.Windows.Data;
using Experiment3.Models;

namespace Experiment3.Helpers
{
  internal class QuantityConverter<T> : IValueConverter
  {
    public object Convert(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      var d = value as QuantityWithMetadata<T>;
      if (d == null)
        return "(null)";

      return d.IsStale ? "Stale" : $"{d.Value:F3}";
    }

    public object ConvertBack(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class DoubleQuantityConverter : QuantityConverter<double>
  {
  }

  internal class ULongQuantityConverter : QuantityConverter<ulong>
  {
  }
}