using Nmea0183.Messages.Enum;

namespace Nmea0183.Messages.Interfaces
{
  public interface IHasPosition
  {
    double Latitude { get; set; }
    NorthSouth LatitudeHemisphere { get; set; }
    double Longitude { get; set; }
    EastWest LongitudeHemisphere { get; set; }
  }
}