using System;
using System.Text.RegularExpressions;
using Geometry;
using Nmea0183.Messages;
using Nmea0183.Messages.Enum;

namespace Nmea0183
{
  public class Position
  {
    private static Regex _regexCoordinateElement = new Regex(@"^(\d+)(\d{2}.\d+)$");
    public double Latitude { get; set; }
    public NorthSouth LatitudeHemisphere { get; set; }
    public double Longitude { get; set; }
    public EastWest LongitudeHemisphere { get; set; }

    public Position()
    {
    }

    public Position(string lat, string ns, string lon, string ew)
    {
      Latitude = ParseNmeaCoordinateElement(lat);
      LatitudeHemisphere = MessageFormatting.ParseOneLetterEnumByValue<NorthSouth>(ns);
      Longitude = ParseNmeaCoordinateElement(lon);
      LongitudeHemisphere = MessageFormatting.ParseOneLetterEnumByValue<EastWest>(ew);
    }

    private static double ParseNmeaCoordinateElement(string s)
    {
      var match = _regexCoordinateElement.Match(s);

      if (!match.Success)
        throw new FormatException("Invalid position in message.");

      int degrees = int.Parse(match.Groups[1].Value);
      double minutes = double.Parse(match.Groups[2].Value);

      return degrees + minutes/60;
    }

    private static string FormatCoordinateElement(double d)
    {
      int degrees = (int) d;
      double minutes = 60*d%60;
      return degrees.ToString("D2") + minutes.ToString("00.####");
    }

    public override string ToString()
    {
      return
        $"{FormatCoordinateElement(Latitude)},{MessageFormatting.F(LatitudeHemisphere)},{FormatCoordinateElement(Longitude)},{MessageFormatting.F(LongitudeHemisphere)}";
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