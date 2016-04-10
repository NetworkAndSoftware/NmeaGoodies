using System;
using System.Globalization;
using Nmea0183.Constants;
using Nmea0183.Messages.Enum;
using Nmea0183.Messages.Interfaces;

// ReSharper disable InconsistentNaming

namespace Nmea0183.Messages
{
  ///<summary>recommended minimum data for gps</summary>
  [CommandName(MessageName.RMC)]
  // ReSharper disable once InconsistentNaming
  public class RMC : MessageBase, IHasPosition, IMightHaveTime, IHasDateAndTime, IHasStatus
  {
    public DateTime? DateTime { get; set; }

    // Implementation of IMightHaveTime
    public TimeSpan? Time
    {
      get { return DateTime?.TimeOfDay; }
      set
      { if (!DateTime.HasValue)
          DateTime = new DateTime();

        DateTime = DateTime.Value.Date + value;
      }
    }

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

    // nmea 2.3
    public FixMode Mode { get; set; }

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
      DateTime = System.DateTime.ParseExact(parts[8], DATETIME_DDMMYY, DateTimeFormatInfo.InvariantInfo,
        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

    if (!string.IsNullOrWhiteSpace(parts[8]))
    {
        DateTime time;

        /* 
              
              if (
                !System.DateTime.TryParseExact(parts[0], HHMMSSfff, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out time))
              {
                time = System.DateTime.ParseExact(parts[0], HHMMSS, DateTimeFormatInfo.InvariantInfo,
                  DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
              }
        */

     time = System.DateTime.ParseExact(parts[0], DATETIME_HHMMSSfff, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

      DateTime = DateTime?.Add(time.TimeOfDay) ?? time;
    }

    Status = ParseOneLetterEnumByValue<Flag>(parts[1]);
    Latitude = float.Parse(parts[2]);
    LatitudeHemisphere = ParseOneLetterEnumByValue<NorthSouth>(parts[3]);
    Longitude = float.Parse(parts[4]);
    LongitudeHemisphere = ParseOneLetterEnumByValue<EastWest>(parts[5]);
    SOG = float.Parse(parts[6]);
    TMG = float.Parse(parts[7]);

    if (!string.IsNullOrWhiteSpace(parts[9]))
      MagneticVariation = float.Parse(parts[9]);

    if (!string.IsNullOrWhiteSpace(parts[10]))
      MagneticVariationDirection = ParseOneLetterEnumByValue<EastWest>(parts[10]);

    if (parts.Length > 11 && !string.IsNullOrWhiteSpace(parts[11]))
      Mode = ParseOneLetterEnumByValue<FixMode>(parts[11]);
  }

  protected override string CommandBody
  {
    get
    {
      string s = 
        $"{FormatTime()},{F(Status)},{Latitude:F3},{F(LatitudeHemisphere)},{Longitude:F3},{F(LongitudeHemisphere)},{SOG:F3},{TMG:F3},{FormatDate()},{MagneticVariation:F3},{F(MagneticVariationDirection)}";
      if (0 != (int) Mode)
        s = s + $",{F(Mode)}";
      return s;
    }
  }

  private string FormatDate()
    {
      return DateTime?.ToString(DATETIME_DDMMYY, DateTimeFormatInfo.InvariantInfo) ?? string.Empty;
    }

    private string FormatTime()
    {
      return DateTime?.ToString(DATETIME_HHMMSSfff, DateTimeFormatInfo.InvariantInfo) ?? string.Empty;
    }

}


}
