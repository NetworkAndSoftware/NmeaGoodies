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

    public GBS(string talkerId) : base(talkerId)
    {
    }

    internal GBS(string talkerId, string[] parts) : base(talkerId)
    {
      throw new System.NotImplementedException();
    }

    protected override string CommandBody { get; }
  }
}
