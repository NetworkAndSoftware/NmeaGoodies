using System;
using Nmea0183.Constants;
using Nmea0183.Messages.Enum;

namespace Nmea0183.Messages
{
  ///<summary>Auto Pilot B sentence</summary>
  [CommandName(MessageName.APB)]
  // ReSharper disable once InconsistentNaming
  public class APB : MessageBase
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
    public APB(string talkerId) : base(talkerId)
    { ArrivalPerpendicular = Flag.Void;
      ArrivalCircular = Flag.Void;
    }

    /// <summary>
    /// Deserializes command 
    /// </summary>
    /// <param name="talkedId"></param>
    /// <param name="parts"></param>
    internal APB(string talkedId, string[] parts) : base(talkedId)
    {
      XTE = double.Parse(parts[2]);
      SteerTurn = MessageFormatting.ParseOneLetterEnumByValue<Turn>(parts[3]);
      XteUnits = MessageFormatting.ParseOneLetterEnumByValue<Units>(parts[4]);
      ArrivalCircular = MessageFormatting.ParseOneLetterEnumByValue<Flag>(parts[5]);
      ArrivalPerpendicular = MessageFormatting.ParseOneLetterEnumByValue<Flag>(parts[6]);
      BOD = double.Parse(parts[7]);
      BodMagneticOrTrue = MessageFormatting.ParseOneLetterEnumByValue<MagneticOrTrue>(parts[8]);
      DestinationWayPointId = int.Parse(parts[9]);
      Bearing = double.Parse(parts[10]);
      BearingMagneticOrTrue = MessageFormatting.ParseOneLetterEnumByValue<MagneticOrTrue>(parts[11]);
      Heading = double.Parse(parts[12]);
      HeadingMagneticOrTrue = MessageFormatting.ParseOneLetterEnumByValue<MagneticOrTrue>(parts[13]);
    }

    public Turn SteerTurn { get; set; }

    public Units XteUnits { get; set; }

    public int DestinationWayPointId { get; set; }
    
    protected override string CommandBody => $"A,A,{XTE:F4},{Convert.ToChar(SteerTurn)},{Convert.ToChar(XteUnits)},{Convert.ToChar(ArrivalCircular)},{Convert.ToChar(ArrivalPerpendicular)},{BOD:F4},{Convert.ToChar(BodMagneticOrTrue)},{DestinationWayPointId:D3},{Bearing:F4},{Convert.ToChar(BearingMagneticOrTrue)},{Heading:F4},{Convert.ToChar(HeadingMagneticOrTrue)}";
  }
}
