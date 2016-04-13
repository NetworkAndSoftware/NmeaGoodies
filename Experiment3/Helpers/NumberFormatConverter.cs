using System;
using System.Globalization;
using System.Windows.Data;

namespace Experiment3.Helpers
{
  public class NumberFormatConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
        return false;

      if (!(value is IFormattable))
        throw new ArgumentException();

      return ((IFormattable) value).ToString(parameter.ToString(), culture.NumberFormat);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
        return null;
      
      if (!targetType.IsInstanceOfType(typeof(IFormattable)))
        throw new ArgumentException();

      throw new NotImplementedException();
    }
  }
}