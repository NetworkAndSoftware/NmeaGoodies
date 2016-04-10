/*GGA - essential fix data which provide 3D location and accuracy data.

 $GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M,,*47

Where:
     GGA          Global Positioning System Fix Data
     123519       Fix taken at 12:35:19 UTC
     4807.038,N   Latitude 48 deg 07.038' N
     01131.000,E  Longitude 11 deg 31.000' E
     1            Fix quality: 0 = invalid
                               1 = GPS fix (SPS)
                               2 = DGPS fix
                               3 = PPS fix
			       4 = Real Time Kinematic
			       5 = Float RTK
                               6 = estimated (dead reckoning) (2.3 feature)
			       7 = Manual input mode
			       8 = Simulation mode
     08           Number of satellites being tracked
     0.9          Horizontal dilution of position
     545.4,M      Altitude, Meters, above mean sea level
     46.9,M       Height of geoid (mean sea level) above WGS84
                      ellipsoid
     (empty field) time in seconds since last DGPS update
     (empty field) DGPS station ID number
     *47          the checksum data, always begins with *
     
 */
 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nmea0183.Constants;
using Nmea0183.Messages.Enum;
using Nmea0183.Messages.Interfaces;

namespace Nmea0183.Messages
{

  ///<summary>Fix information</summary>
  [CommandName(MessageName.GGA)]
  // ReSharper disable once InconsistentNaming
  public class GGA : MessageBase, IHasPosition, IMightHaveTime
  {
    public TimeSpan? Time { get; set; }
    public double Latitude { get; set; }
    public NorthSouth LatitudeHemisphere { get; set; }
    public double Longitude { get; set; }
    public EastWest LongitudeHemisphere { get; set; }
    public FixQuality FixQuality { get; set; }
    public int TrackedSatelliteCount { get; set; }
    public float HorizontalDilution { get; set; }
    public float Altitude { get; set; }
    public AltitudeUnit AltitudeUnit { get; set; }
    public float MeanSeaLevel { get; set; }
    public AltitudeUnit MeanSeaLevelUnit { get; set; }
    public TimeSpan? TimeSinceLastDGPSFix { get; set; }
    public int DGPSStationId { get; set; }

    public GGA(string talkerId) : base(talkerId)
    {

    }

    internal GGA(string talkerId, string[] parts) : base(talkerId)
    {
      Time = TimeSpan.ParseExact(parts[0], TIMESPAN_HHMMSSfff, DateTimeFormatInfo.InvariantInfo);
      Latitude = float.Parse(parts[1]);
      LatitudeHemisphere = ParseOneLetterEnumByValue<NorthSouth>(parts[2]);
      Longitude = float.Parse(parts[3]);
      LongitudeHemisphere = ParseOneLetterEnumByValue<EastWest>(parts[4]);
      FixQuality = ParseOneLetterEnumByValue<FixQuality>(parts[5]);
      TrackedSatelliteCount = int.Parse(parts[6]);
      HorizontalDilution = float.Parse(parts[7]);
      Altitude = float.Parse(parts[8]);
      AltitudeUnit = ParseOneLetterEnumByValue<AltitudeUnit>(parts[9]);
      MeanSeaLevel = float.Parse(parts[10]);
      MeanSeaLevelUnit = ParseOneLetterEnumByValue<AltitudeUnit>(parts[11]);
      if (!string.IsNullOrWhiteSpace(parts[12]))
        TimeSinceLastDGPSFix = TimeSpan.FromSeconds(int.Parse(parts[12]));
      if (!string.IsNullOrWhiteSpace(parts[13]))
        DGPSStationId = int.Parse(parts[13]);
    }

    protected override string CommandBody
    {
      get
      {
        var parts = new List<string>
        {
          FormatTime(),
          $"{Latitude:F3},{F(LatitudeHemisphere)},{Longitude: F3},{F(LongitudeHemisphere)}",
          F(FixQuality),
          $"{TrackedSatelliteCount:D2}",
          $"{HorizontalDilution:F1}",
          $"{Altitude:F1}",
          F(AltitudeUnit),
          $"{MeanSeaLevel:F1}",
          F(MeanSeaLevelUnit),
          TimeSinceLastDGPSFix.HasValue ? ((int) TimeSinceLastDGPSFix.Value.TotalSeconds).ToString() : string.Empty,
          DGPSStationId.ToString()
        };

        return string.Join(",", parts);
      }

    }

    private string FormatTime()
    {
      return Time?.ToString(DATETIME_HHMMSSfff) ?? string.Empty;
    }
  }
}
