using System;
using System.Globalization;
using Nmea0183.Constants;

// ReSharper disable InconsistentNaming

namespace Nmea0183.Messages
{ ///<summary>recommended minimum data for gps</summary>
  [CommandName(MessageName.RMC)]
  // ReSharper disable once InconsistentNaming
  public class RMC : MessageBase
  {
    private const string DDMMYY = "ddMMyy";
    private const string HHMMSS = "HHmmss";
    public DateTime? DateTime { get; set; }
    public Flag Status { get; set; }      // Void indicates receiver error
    public double Latitude { get; set; }
    public NorthSouth LatitudeHemisphere { get; set; }
    public double Longitude { get; set; }
    public EastWest LongitudeHemisphere { get; set; }
    /// <summary>
    /// Speed over ground in knots
    /// </summary>
    public double SOG { get; set; } 
    /// <summary>
    /// Track made good, in degrees True
    /// </summary>
    public double TMG { get; set; }
    public double MagneticVariation { get; set; }
    public EastWest MagneticVariationDirection { get; set; }

    public RMC(string talkerId) : base(talkerId)
    { 
    }

  /// <summary>
  /// Deserializes command 
  /// </summary>
  /// <param name="talkedId"></param>
  /// <param name="parts"></param>
  internal RMC(string talkedId, string[] parts) : base(talkedId)
    {
      if (!string.IsNullOrWhiteSpace(parts[8]))
        DateTime = System.DateTime.ParseExact(parts[8], DDMMYY, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

      if (!string.IsNullOrWhiteSpace(parts[8]))
      {
        var time = System.DateTime.ParseExact(parts[0], HHMMSS, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

        DateTime = DateTime?.Add(time.TimeOfDay) ?? time;
      }

      Status = ParseOneLetterEnumByValue<Flag>(parts[1]);
      Latitude = float.Parse(parts[2]);
      LatitudeHemisphere = ParseOneLetterEnumByValue<NorthSouth>(parts[3]);
      Longitude = float.Parse(parts[4]);
      LongitudeHemisphere = ParseOneLetterEnumByValue<EastWest>(parts[5]);
      SOG = float.Parse(parts[6]);
      TMG = float.Parse(parts[7]);
      MagneticVariation = float.Parse(parts[9]);
      MagneticVariationDirection = ParseOneLetterEnumByValue<EastWest>(parts[10]);
    }

    protected override string CommandBody => $"{FormatTime()},{F(Status)},{Latitude:F3},{F(LatitudeHemisphere)},{Longitude:F3},{F(LongitudeHemisphere)},{SOG:F3},{TMG:F3},{FormatDate()},{MagneticVariation:F3},{F(MagneticVariationDirection)}";

    private string FormatDate()
    {
      return DateTime?.ToString(DDMMYY, DateTimeFormatInfo.InvariantInfo) ?? string.Empty;
    }

    private string FormatTime()
    {
      return DateTime?.ToString(HHMMSS, DateTimeFormatInfo.InvariantInfo) ?? string.Empty;
    }
  }


}
