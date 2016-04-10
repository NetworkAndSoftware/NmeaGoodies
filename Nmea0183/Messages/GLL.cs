/*
GLL - Geographic Latitude and Longitude is a holdover from Loran data and some old units may not send the time and data active information if they are emulating Loran data. If a gps is emulating Loran data they may use the LC Loran prefix instead of GP.

  $GPGLL,4916.45,N,12311.12,W,225444,A,*1D

Where:
     GLL          Geographic position, Latitude and Longitude
     4916.46,N    Latitude 49 deg. 16.45 min. North
     12311.12,W   Longitude 123 deg. 11.12 min. West
     225444       Fix taken at 22:54:44 UTC
     A            Data Active or V (void)
     *iD          checksum data
*/


using System;
using System.Globalization;
using Nmea0183.Constants;
using Nmea0183.Messages.Enum;
using Nmea0183.Messages.Interfaces;

namespace Nmea0183.Messages
{
  ///<summary>Lat/Lon data</summary>
  [CommandName(MessageName.GLL)]
  // ReSharper disable once InconsistentNaming
  public class GLL : MessageBase, IHasPosition, IMightHaveTime, IHasStatus
  {
    public double Latitude { get; set; }
    public NorthSouth LatitudeHemisphere { get; set; }
    public double Longitude { get; set; }
    public EastWest LongitudeHemisphere { get; set; }
    public TimeSpan? Time { get; set; }
    public Flag Status { get; set; }

    public GLL(string talkerId) : base(talkerId)
    {
    }

    internal GLL(string talkerId, string[] parts) : base(talkerId)
    {
      Latitude = float.Parse(parts[0]);
      LatitudeHemisphere = ParseOneLetterEnumByValue<NorthSouth>(parts[1]);
      Longitude = float.Parse(parts[2]);
      LongitudeHemisphere = ParseOneLetterEnumByValue<EastWest>(parts[3]);
      Time = TimeSpan.ParseExact(parts[4], TIMESPAN_HHMMSSfff, DateTimeFormatInfo.InvariantInfo);
      Status = ParseOneLetterEnumByValue<Flag>(parts[5]);
    }

    protected override string CommandBody => $"{Latitude:F3},{F(LatitudeHemisphere)},{Longitude:F3},{F(LongitudeHemisphere)},{FormatTime()},{F(Status)}";

    private string FormatTime()
    {
      return Time?.ToString(DATETIME_HHMMSSfff) ?? string.Empty;
    }

  }
}
