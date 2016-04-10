// ReSharper disable InconsistentNaming
namespace Nmea0183.Messages.Enum
{
  public enum Turn
  {
    Right = 'R',
    Left = 'L',
  };

  public enum MagneticOrTrue
  {
    Magnetic = 'M',
    True = 'T'
  };

  public enum NorthSouth
  {
    North = 'N',
    South = 'S'
  };

  public enum Units
  {
    NauticalMiles = 'N',
    KiloMeters = 'K'
  };

  public enum EastWest
  {
    East = 'E',
    West = 'W'
  };

  public enum Flag
  {
    Void = 'V',
    Active = 'A'
  }

  public enum FixMode
  {
    Autonomous = 'A',
    Differential = 'D',
    Estimated = 'E',
    NotValid = 'N',
    Simulator = 'S'
  }

  public enum FixQuality
  {///<summary>invalid</summary>
    Invalid = 0,
    ///<summary>GPS fix(SPS)</summary>
    SPS = 1,
    ///<summary>DGPS fix</summary>
    DGPS = 2,
    ///<summary>PPS fix</summary>
    PPS = 3,
    ///<summary>Real Time Kinematic</summary>
    Kinematic = 4,
    ///<summary>Float RTK</summary>
    RTK = 5,
    ///<summary>estimated(dead reckoning) (2.3 feature)</summary>
    Estimated = 6,
    ///<summary>Manual input mode</summary>
    Manual = 7,
    ///<summary>Simulation mode</summary>
    Simulation = 8,
  }

  public enum AltitudeUnit
  {
    Meters = 'M'
  }

}
