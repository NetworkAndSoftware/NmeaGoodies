using System;
using System.Collections.Generic;
using Nmea0183.Constants;
using Nmea0183.Messages.Enum;

namespace Nmea0183.Messages
{
  ///<summary>
  /// HDG - Heading - Deviation & Variation
  ///         1   2   3 4   5 6
  ///         |   |   | |   | |
  ///  $--HDG,x.x,x.x,a,x.x,a* hh
  /// Field Number:
  /// 
  /// 1.Magnetic Sensor heading in degrees
  /// 2.Magnetic Deviation, degrees
  /// 3.Magnetic Deviation direction, E = Easterly, W = Westerly
  /// 4.Magnetic Variation degrees
  /// 5.Magnetic Variation direction, E = Easterly, W = Westerly
  ///</summary>
  [CommandName(MessageName.HDG)]
  // ReSharper disable once InconsistentNaming
  public class HDG : MessageBase, IHaveHeading
  {
    public IMessageCompassValue Heading { get; set; }
    public MagneticContext MagneticContext { get; set; }


    public HDG(string talkerId) : base(talkerId)
    {
    }

    internal HDG(string talkedId, string[] parts) : base(talkedId)
    {
      Heading = MessageCompassValueFactory.FromMessageParts(parts[0], true);
      if (string.IsNullOrWhiteSpace(parts[3]))
        return;

      double? deviation = null;

      if (!string.IsNullOrWhiteSpace(parts[1]))
      {
        deviation = GetCompassChange(parts[1], parts[2]);
      }

      MagneticContext = new MagneticContext(GetCompassChange(parts[3], parts[4]), deviation);
    }

    private static double GetCompassChange(string scalarpart, string directionpart)
    {
      var eastwest = MessageFormatting.ParseOneLetterEnumByValue<EastWest>(directionpart);
      var scalar = double.Parse(scalarpart);
      return EastWest.East == eastwest ? -scalar : scalar;
    }

    protected override string CommandBody => $"{Heading.Value:F3},{FormatContext()}";

    private string FormatContext()
    {
      var parts = new[]
      {
        MagneticContext?.Deviation == null ? "," : FormatAsMagneticChange(MagneticContext.Deviation.Value),
        MagneticContext == null ? "," : FormatAsMagneticChange(MagneticContext.Declination)
      };
      var contesxtparts = string.Join(",", parts);
      return contesxtparts;
    }

    private static string FormatAsMagneticChange(double d)
    {
      return $"{Math.Abs(d).ToString("F3")},{(d > 0 ? 'W' : 'E')}";
    }
  }
}