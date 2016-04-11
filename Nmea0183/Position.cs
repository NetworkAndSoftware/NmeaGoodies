using System;
using Geometry;
using Nmea0183.Messages;
using Nmea0183.Messages.Enum;

namespace Nmea0183
{
  public class Position
  {
    public double Latitude { get; set; }
    public NorthSouth LatitudeHemisphere { get; set; }
    public double Longitude { get; set; }
    public EastWest LongitudeHemisphere { get; set; }

    public Position()
    {
    }

    public Position(string lat, string ns, string lon, string ew)
    {
      Latitude = double.Parse(lat) / 100;
      LatitudeHemisphere = MessageFormatting.ParseOneLetterEnumByValue<NorthSouth>(ns);
      Longitude = double.Parse(lon) / 100;
      LongitudeHemisphere = MessageFormatting.ParseOneLetterEnumByValue<EastWest>(ew);
    }

    public override string ToString()
    {
      return
        $"{(100 * Latitude):F3},{MessageFormatting.F(LatitudeHemisphere)},{(100 * Longitude): F3},{MessageFormatting.F(LongitudeHemisphere)}";
    }

    public static implicit operator Coordinate(Position position)
    {
      var latitude = (position.LatitudeHemisphere == NorthSouth.North ? position.Latitude : -position.Latitude);
      var longitude = (position.LongitudeHemisphere == EastWest.East ? position.Longitude : -position.Longitude);
      var coordinate = new Coordinate(Geometry.Latitude.FromDegrees(latitude), Geometry.Longitude.FromDegrees(longitude));
      return coordinate;
    }

    public static implicit operator Position(Coordinate coordinate)
    {
      var position = new Position()
      {
        Latitude = coordinate.Latitude.Hemisphere == Geometry.Latitude.HemisphereType.North ? coordinate.Latitude.Degrees : -coordinate.Latitude.Degrees,
        LatitudeHemisphere = coordinate.Latitude.Hemisphere == Geometry.Latitude.HemisphereType.North ? NorthSouth.North : NorthSouth.South,
        Longitude = Math.Abs(coordinate.Longitude.Degrees),
        LongitudeHemisphere = coordinate.Longitude.Degrees > 0 ? EastWest.East : EastWest.West
      };
      return position;
    }
  }
}