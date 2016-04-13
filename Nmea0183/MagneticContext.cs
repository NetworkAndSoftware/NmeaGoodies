using System;

namespace Nmea0183
{
  /// <summary>
  ///   Allows conversions of Magnetic and True bearings
  /// </summary>
  public class MagneticContext
  {
    private readonly double _magneticdeclination;

    public MagneticContext(double magneticdeclination)
    {
      _magneticdeclination = magneticdeclination;
    }

    public TrueMessageCompassValue True(IMessageCompassValue value)
    {
      if (value is TrueMessageCompassValue)
        return (TrueMessageCompassValue) value;
      if (value is MagneticMessageCompassValue)
        return new TrueMessageCompassValue(value.Value - _magneticdeclination);
      return null;
    }

    public MagneticMessageCompassValue Magnetic(IMessageCompassValue value)
    {
      if (value is MagneticMessageCompassValue)
        return (MagneticMessageCompassValue) value;
      if (value is TrueMessageCompassValue)
        return new MagneticMessageCompassValue(value.Value + _magneticdeclination);
      return null;
    }

    public delegate double Function2(double value1, double value2);

    public double ExecuteFunction(IMessageCompassValue value1, IMessageCompassValue value2, Function2 op)
    {
      double value;

      if (value1 is MagneticMessageCompassValue)
        value = True(value2).Value;
      else if (value1 is TrueMessageCompassValue)
        value = Magnetic(value2).Value;
      else
        throw new InvalidOperationException();

      return op(value1.Value, value);
    }

    public delegate double Function(double value1);

    public IMessageCompassValue ExecuteFunction(IMessageCompassValue value1, Function op)
    {
      var value = op(value1.Value);

      if (value1 is MagneticMessageCompassValue)
        return new MagneticMessageCompassValue(value);
      if (value1 is TrueMessageCompassValue)
        return new TrueMessageCompassValue(value);
      throw new InvalidOperationException();
    }

  }
}