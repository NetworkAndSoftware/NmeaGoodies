/*
    From: http://www.trimble.com/OEM_ReceiverHelp/V4.44/en/NMEA-0183messages_GBS.html 

    NMEA-0183 message: GBS

    Related Topics
    NMEA-0183 messages: Overview
    GNSS satellite fault detection (RAIM support)
    An example of the GBS message string is:

    $GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D

    GBS message fields
    Field	Meaning
    0	Message ID $--GBS
    1	UTC of position fix
    2	Expected error in latitude, in meters, due to bias, with noise = 0
    3	Expected error in longitute, in meters, due to bias, with noise = 0
    4	Expected error in altitude, in meters, due to bias, with noise = 0
    5	ID number of most likely failed satellite
    6	Probability of missed detection of most likely failed satellite
    7	Estimate of bias, in meters, on the most likely failed satellite
    8	Standard deviation of bias estimate
    9	The checksum data, always begins with *

*/


using System;
using System.Collections.Generic;
using System.Globalization;
using Nmea0183.Constants;
using Nmea0183.Messages.Interfaces;

namespace Nmea0183.Messages
{
  /// <summary>
  /// GNSS satellite fault detection (RAIM support) 
  /// (according to http://www.trimble.com/OEM_ReceiverHelp/V4.44/en/NMEA-0183messages_GBS.html )
  /// </summary>
  [CommandName(MessageName.GBS)]
  // ReSharper disable once InconsistentNaming
  public class GBS : MessageBase, IMightHaveTime
  {
    public TimeSpan? Time { get; set; }
    public double? LatitudeError { get; set; }
    public double? LongitudeError { get; set; }
    public int? MostLikelyFailedSatelliteId { get; set; }
    public double? MissedDetectionProbability { get; set; }
    public double? BiasEstimate { get; set; }
    public double? BiasEstimateStandardDeviation { get; set; }


    public GBS(string talkerId) : base(talkerId)
    {
    }

    internal GBS(string talkerId, string[] parts) : base(talkerId)
    {
      Time = TimeSpan.ParseExact(parts[0], TIMESPAN_HHMMSSfff, DateTimeFormatInfo.InvariantInfo);
      if (!string.IsNullOrWhiteSpace(parts[1]))
        LatitudeError = double.Parse(parts[1]);
      if (!string.IsNullOrWhiteSpace(parts[2]))
        LongitudeError = double.Parse(parts[2]);
      if (!string.IsNullOrWhiteSpace(parts[3]))
        MissedDetectionProbability = double.Parse(parts[3]);
      if (!string.IsNullOrWhiteSpace(parts[4]))
        BiasEstimate = double.Parse(parts[4]);
      if (!string.IsNullOrWhiteSpace(parts[5]))
        BiasEstimateStandardDeviation = double.Parse(parts[5]);
    }

    protected override string CommandBody
    {
      get
      {
        var parts = new List<string>
        {
          FormatTime(),
          LatitudeError.HasValue? LatitudeError.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
          LongitudeError.HasValue? LongitudeError.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
          MostLikelyFailedSatelliteId.HasValue ? MostLikelyFailedSatelliteId.ToString() : string.Empty,
          MissedDetectionProbability.HasValue ? MissedDetectionProbability.ToString() : string.Empty,
          BiasEstimate.HasValue ? BiasEstimate.ToString() : string.Empty,
          BiasEstimateStandardDeviation.HasValue ? BiasEstimateStandardDeviation.ToString() : string.Empty
        };

        return string.Join(",", parts);
      }
    }

    private string FormatTime()
    {
      return Time?.ToString(TIMESPAN_HHMMSSfff) ?? string.Empty;
    }
  }
}
