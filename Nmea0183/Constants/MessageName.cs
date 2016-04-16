// ReSharper disable InconsistentNaming

namespace Nmea0183.Constants
{
  /// <summary>
  /// From http://freenmea.net/docs/nmea0183
  /// and http://www.catb.org/gpsd/NMEA.html#_hdm_heading_magnetic
  /// </summary>
  public enum MessageName
  {
    Unknown = 0,

    ///<summary>Waypoint Arrival Alarm</summary>
    AAM,

    ///<summary>Almanac data</summary>
    ALM,

    ///<summary>Auto Pilot A sentence</summary>
    APA,

    ///<summary>Auto Pilot B sentence</summary>
    APB,

    ///<summary>Bearing Origin to Destination</summary>
    BOD,

    ///<summary>Bearing using Great Circle route</summary>
    BWC,

    ///<summary>Datum being used.</summary>
    DTM,

    ///<summary>Fix information</summary>
    GGA,

    ///<summary>Lat/Lon data</summary>
    GLL,

    ///<summary>GPS Range Residuals</summary>
    GRS,

    ///<summary>Overall Satellite data</summary>
    GSA,

    ///<summary>GPS Pseudorange Noise Statistics</summary>
    GST,

    ///<summary>Detailed Satellite data</summary>
    GSV,

    ///<summary>send control for a beacon receiver</summary>
    MSK,

    ///<summary>Beacon receiver status information.</summary>
    MSS,

    ///<summary>recommended Loran data</summary>
    RMA,

    ///<summary>recommended navigation data for gps</summary>
    RMB,

    ///<summary>recommended minimum data for gps</summary>
    RMC,

    ///<summary>route message</summary>
    RTE,

    ///<summary>Transit Fix Data</summary>
    TRF,

    ///<summary>Multiple Data ID</summary>
    STN,

    ///<summary>dual Ground / Water Spped</summary>
    VBW,

    ///<summary>Vector track an Speed over the Ground</summary>
    VTG,

    ///<summary>Waypoint closure velocity (Velocity Made Good)</summary>
    WCV,

    ///<summary>Waypoint Location information</summary>
    WPL,

    ///<summary>cross track error</summary>
    XTC,

    ///<summary>measured cross track error</summary>
    XTE,

    ///<summary>Zulu(UTC) time and time to go(to destination)</summary>
    ZTG,

    ///<summary>Date and Time</summary>
    ZDA,
    // Some gps receivers with special capabilities output these special messages.

    ///<summary>Compass output</summary>
    HCHDG,

    ///<summary>Remote Control for a DGPS receiver</summary>
    PSLIB,

    /// <summary>
    /// Heading, Magnetic. 
    /// See http://www.catb.org/gpsd/NMEA.html#_hdm_heading_magnetic
    /// </summary>
    HDM,

    /// <summary>
    /// Heading, True. 
    /// See http://www.catb.org/gpsd/NMEA.html#_hdm_heading_magnetic
    /// </summary>
    HDT,

    /// <summary>
    /// GNSS satellite fault detection (RAIM support) 
    /// (according to http://www.trimble.com/OEM_ReceiverHelp/V4.44/en/NMEA-0183messages_GBS.html )
    /// </summary>
    GBS,
  };
}
