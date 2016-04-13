using System;
using System.Windows.Data;
using Experiment3.Models;
using Nmea0183;

namespace Experiment3.Helpers
{
  internal abstract class QuantityWithCompassValueConverter<T> : BaseWithMagneticContext, IValueConverter
    where T : IMessageCompassValue
  {
    public object Convert(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      var quantityWithMetadata = value as QuantityWithMetadata<T>;
      if (quantityWithMetadata == null)
        return "(null)";

      return quantityWithMetadata.IsStale ? "Stale" : $"{Convert(quantityWithMetadata.Value).Value:F1}";
    }

    protected abstract IMessageCompassValue Convert(IMessageCompassValue compassValue);

    public object ConvertBack(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }

  internal class QuantityWithCompassValueAsTrueConverter : QuantityWithCompassValueConverter<IMessageCompassValue>
  {
    protected override IMessageCompassValue Convert(IMessageCompassValue compassValue)
    {
      return
        MagneticContext.True(compassValue);
    }
  }

  internal class QuantityWithCompassValueAsMagneticConverter : QuantityWithCompassValueConverter<IMessageCompassValue>
  {
    protected override IMessageCompassValue Convert(IMessageCompassValue compassValue)
    {
      return
        MagneticContext.Magnetic(compassValue);
    }
  }
}