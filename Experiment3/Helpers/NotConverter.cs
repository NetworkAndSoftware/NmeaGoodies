using System;
using System.Globalization;
using System.Windows.Data;

namespace Experiment3.Helpers
{
  public class NotConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
      if (value is bool)
      {
        return !(bool)value;
      }
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
      if (value is bool)
      {
        return !(bool)value;
      }
      return value;
    }

    #endregion
  }

}
