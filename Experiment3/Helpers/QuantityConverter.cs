using System;
using System.Windows.Data;
using Experiment3.Models;

namespace Experiment3.Helpers
{
  internal abstract class QuantityConverter<T> : IValueConverter
  {
    public object Convert(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      var d = value as QuantityWithMetadata<T>;
      if (d == null)
        return "(null)";

      return d.IsStale ? "Stale" : Format(d, (string) parameter);
    }

    protected abstract string Format(T d, string format);

    public object ConvertBack(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class DoubleQuantityConverter : QuantityConverter<double>
  {
    protected override string Format(double d, string format)
    {
      return null == format ? $"{d:F3}" : d.ToString(format);
    }
  }

  internal class ULongQuantityConverter : QuantityConverter<ulong>
  {
    protected override string Format(ulong d, string format)
    {
      return null == format ? $"{d:F3}" : d.ToString(format);
    }

  }
}