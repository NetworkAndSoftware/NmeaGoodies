using System;
using System.ComponentModel;
using System.Linq;

namespace Nmea0183
{
  public class Apb : MessageBase
  {
    public double BOD { get; set; }
    
    public MagneticOrTrue BodMagneticOrTrue { get; set; }

    public double XTE { get; set; }
    public double Bearing { get; set; }

    public MagneticOrTrue BearingMagneticOrTrue { get; set; }

    public double Heading { get; set; }

    public MagneticOrTrue HeadingMagneticOrTrue { get; set; }

    public enum Direction
    {
      Right,
      Left,
    };

    public Apb(string talkerId) : base(talkerId, "APB")
    {
    }

    private string ToString(Direction direction)
    {
      switch (direction)
      {
        case Direction.Right:
          return "R";
        case Direction.Left:
          return "L";
        default:
          throw new ArgumentOutOfRangeException("direction", direction, null);
      }
    }

    public Direction SteerDirection { get; set; }

    public enum Units
    {
      NauticalMiles = 'N',
      KiloMeters = 'K'
    };

    public Units XteUnits { get; set; }

    public enum MagneticOrTrue
    {
      Magnetic = 'M',
      True = 'T'
    };


    public int DestinationWayPointId { get; set; }


    protected override string CommandBody
    {
      get
      {
        return string.Format("A,A,{0:F4},{1},{2},V,V,{3:F4},{4},{5:D3},{6:F4},{7},{8:F4},{9}", XTE, ToString(SteerDirection),
          Convert.ToChar(XteUnits), BOD, Convert.ToChar(BodMagneticOrTrue), DestinationWayPointId, Bearing,
          Convert.ToChar(BearingMagneticOrTrue), Heading, Convert.ToChar(HeadingMagneticOrTrue));
      }
    }
  }

  }
