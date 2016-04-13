using Nmea0183;

namespace Experiment3.Helpers
{
  /// <summary>
  ///  TODO: Ugh
  /// 
  ///  I don't know any other way to insert the MagneticContext into the IValueConverters. 
  /// </summary>
  internal class BaseWithMagneticContext
  {
    public static MagneticContext MagneticContext { get; set; }
  }
}
