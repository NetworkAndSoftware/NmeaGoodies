using System;
using Nmea0183.Constants;

namespace Nmea0183.Messages
{

  [CommandName(MessageNames.APB)]
  public class Apb : MessageBase
  {
    public double BOD { get; set; }
    
    public MagneticOrTrue BodMagneticOrTrue { get; set; }

    public double XTE { get; set; }
    public double Bearing { get; set; }

    public MagneticOrTrue BearingMagneticOrTrue { get; set; }

    public double Heading { get; set; }

    public MagneticOrTrue HeadingMagneticOrTrue { get; set; }

    public Flag ArrivalPerpendicular { get; set; }
    public Flag ArrivalCircular { get; set; }

    /// <summary>
    /// Creates empty, invalid APB command
    /// </summary>
    /// <param name="talkerId"></param>
    public Apb(string talkerId) : base(talkerId)
    { ArrivalPerpendicular = Flag.Void;
      ArrivalCircular = Flag.Void;
    }

    /// <summary>
    /// Deserializes command 
    /// </summary>
    /// <param name="talkedId"></param>
    /// <param name="parts"></param>
    internal Apb(string talkedId, string[] parts) : base(talkedId)
    {
      XTE = double.Parse(parts[2]);
      SteerTurn = ParseEnum<Turn>(parts[3]);
      XteUnits = ParseEnum<Units>(parts[4]);
      ArrivalCircular = ParseEnum<Flag>(parts[5]);
      ArrivalPerpendicular = ParseEnum<Flag>(parts[6]);
      BOD = double.Parse(parts[7]);
      BodMagneticOrTrue = ParseEnum<MagneticOrTrue>(parts[8]);
      DestinationWayPointId = int.Parse(parts[9]);
      Bearing = double.Parse(parts[10]);
      BearingMagneticOrTrue = ParseEnum<MagneticOrTrue>(parts[11]);
      Heading = double.Parse(parts[12]);
      HeadingMagneticOrTrue = ParseEnum<MagneticOrTrue>(parts[13]);
    }

    public Turn SteerTurn { get; set; }

    public Units XteUnits { get; set; }

    public int DestinationWayPointId { get; set; }
    
    protected override string CommandBody => $"A,A,{XTE:F4},{Convert.ToChar(SteerTurn)},{Convert.ToChar(XteUnits)},{Convert.ToChar(ArrivalCircular)},{Convert.ToChar(ArrivalPerpendicular)},{BOD:F4},{Convert.ToChar(BodMagneticOrTrue)},{DestinationWayPointId:D3},{Bearing:F4},{Convert.ToChar(BearingMagneticOrTrue)},{Heading:F4},{Convert.ToChar(HeadingMagneticOrTrue)}";
  }
}
