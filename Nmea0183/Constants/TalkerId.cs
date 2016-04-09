// ReSharper disable InconsistentNaming

namespace Nmea0183.Constants
{
  /// <summary>
  /// From http://freenmea.net/docs/nmea0183
  /// </summary>
  public enum TalkerId
  { /// <summary>GLONASS Receiver</summary>
    GL,
    /// <summary>Global Positioning System(GPS)</summary>
    GP,
    /// <summary>Heading Track Controller(Autopilot) - Magnetic</summary>
    AG,
    /// <summary>Heading Track Controller(Autopilot) - Magnetic</summary>
    AP,
    /// <summary>Automatic Identification System</summary>
    AI,
    /// <summary>Digital Selective Calling(DSC)</summary>
    CD,
    /// <summary>Data Receiver</summary>
    CR,
    /// <summary>Satellite</summary>
    CS,
    /// <summary>Radio-Telephone(MF/HF)</summary>
    CT,
    /// <summary>Radio-Telephone(VHF)</summary>
    CV,
    /// <summary>Scanning Receiver</summary>
    CX,
    /// <summary>DECCA Navigator</summary>
    DE,
    /// <summary>Direction Finder</summary>
    DF,
    /// <summary>Electronic Chart System(ECS)</summary>
    EC,
    /// <summary>Electronic Chart Display & Information System(ECDIS)</summary>
    EI,
    /// <summary>Emergency Position Indicating Beacon(EPIRB)</summary>
    EP,
    /// <summary>Engine room Monitoring Systems</summary>
    ER,
    /// <summary>Global Navigation Satellite System(GNSS)</summary>
    GN,
    /// <summary>HEADING SENSORS: Compass, Magnetic</summary>
    HC,
    /// <summary>Gyro, North Seeking</summary>
    HE,
    /// <summary>Gyro, Non-North Seeking</summary>
    HN,
    /// <summary>Integrated Instrumentation</summary>
    II,
    /// <summary>Integrated Navigation</summary>
    IN,
    /// <summary>Loran C</summary>
    LC,
    /// <summary>Proprietary Code</summary>
    P,
    /// <summary>Radar and/or Radar Plotting</summary>
    RA,
    /// <summary>Sounder, depth</summary>
    SD,
    /// <summary>Electronic Positioning System, other/general</summary>
    SN,
    /// <summary>Sounder, scanning</summary>
    SS,
    /// <summary>Turn Rate Indicator</summary>
    TI,
    /// <summary>VELOCITY SENSORS: Doppler, other/general</summary>
    VD,
    /// <summary>Speed Log, Water, Magnetic</summary>
    VM,
    /// <summary>Speed Log, Water, Mechanical</summary>
    VW,
    /// <summary>Voyage Data Recorder</summary>
    VR,
    /// <summary>Transducer</summary>
    YX,
    /// <summary>TIMEKEEPERS, TIME/DATE: Atomic Clock</summary>
    ZA,
    /// <summary>Chronometer</summary>
    ZC,
    /// <summary>Quartz</summary>
    ZQ,
    /// <summary>Radio Update</summary>
    ZV,
    /// <summary>Weather Instruments</summary>
    WI,
  }
}
