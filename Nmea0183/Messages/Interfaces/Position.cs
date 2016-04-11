using Nmea0183.Messages.Enum;

namespace Nmea0183.Messages.Interfaces
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
      Latitude = double.Parse(lat);
      LatitudeHemisphere = MessageBase.ParseOneLetterEnumByValue<NorthSouth>(ns);
      Longitude = double.Parse(lon);
      LongitudeHemisphere = MessageBase.ParseOneLetterEnumByValue<EastWest>(ew);
    }

    public override string ToString()
    {
      return $"{Latitude:F3},{MessageBase.F(LatitudeHemisphere)},{Longitude: F3},{MessageBase.F(LongitudeHemisphere)}";
    }
  }
}