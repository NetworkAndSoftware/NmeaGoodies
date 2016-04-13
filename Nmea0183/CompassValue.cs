using System.CodeDom;

namespace Nmea0183
{
  public interface ICompassValue<T>
  {
    T Value { get; set; }
    bool IsMagnetic { get; }
  }

  public class TrueCompassValue<T> : ICompassValue<T>
  {
    public TrueCompassValue(T value)
    {
      Value = value;
    }

    public T Value { get; set; }
    public bool IsMagnetic => false;

    public T True
    {
      get { return Value; }
      set { Value = value; }
    }
  }

  public class MagneticCompassValue<T> : ICompassValue<T>
  {
    public MagneticCompassValue(T value)
    {
      Value = value;
    }

    public T Value { get; set; }
    public bool IsMagnetic => true;

  }

  public interface IMessageCompassValue : ICompassValue<double>
  {
    string ToString(string format);
  }

  public class TrueMessageCompassValue : TrueCompassValue<double>, IMessageCompassValue
  {
    public TrueMessageCompassValue(double value) : base(value)
    {
    }

    public override string ToString()
    {
      return $"{Value:F3},T";
    }

    public string ToString(string format)
    {
      var s = Value.ToString(format);
      return $"{s},T";
    }

    public static implicit operator double(TrueMessageCompassValue value)
    {
      return value.Value;
    }

    public static implicit operator TrueMessageCompassValue(double value)
    {
      return new TrueMessageCompassValue(value);
    }
  }

  public class MagneticMessageCompassValue : MagneticCompassValue<double>, IMessageCompassValue
  {
    public MagneticMessageCompassValue(double value) : base(value)
    {
    }

    public string ToString(string format)
    {
      var s = Value.ToString(format);
      return $"{s},M";
    }

  }

  public static class MessageCompassValueFactory 
  {
    private enum MagneticOrTrue
    {
      Magnetic = 'M',
      True = 'T'
    };

    private static IMessageCompassValue Create(double value, MagneticOrTrue magneticortrue)
    {
      switch (magneticortrue)
      {
        case MagneticOrTrue.Magnetic:
          return new MagneticMessageCompassValue(value);
        case MagneticOrTrue.True:
          return new TrueMessageCompassValue(value);
        default:
          return null;
      }
    }

    public static IMessageCompassValue FromMessageParts(string svalue, string smagneticortrue)
    {
      return Create(double.Parse(svalue), MessageFormatting.ParseOneLetterEnumByValue<MagneticOrTrue>(smagneticortrue));
    }
  }
}