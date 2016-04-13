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
    public IMessageCompassValue BOD { get; set; }
    public double XTE { get; set; }
    public IMessageCompassValue Bearing { get; set; }
    public IMessageCompassValue Heading { get; set; }
    public Flag ArrivalPerpendicular { get; set; }
    public Flag ArrivalCircular { get; set; }

    /// <summary>
    /// Creates empty, invalid APB command
    /// </summary>
    /// <param name="talkerId"></param>
    public APB(string talkerId) : base(talkerId)
    { ArrivalPerpendicular = Flag.Void;
      ArrivalCircular = Flag.Void;
      XteUnits = Units.NauticalMiles;
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
      BOD = MessageCompassValueFactory.FromMessageParts(parts[7], parts[8]);
      DestinationWayPointId = int.Parse(parts[9]);
      Bearing = MessageCompassValueFactory.FromMessageParts(parts[10], parts[11]);
      Heading = MessageCompassValueFactory.FromMessageParts(parts[12], parts[13]);
    }

    public Turn SteerTurn { get; set; }

    public Units XteUnits { get; set; }

    public int DestinationWayPointId { get; set; }
    
    protected override string CommandBody
    {
      get
      {
        return
          $"A,A,{XTE:F4},{Convert.ToChar(SteerTurn)},{Convert.ToChar(XteUnits)},{Convert.ToChar(ArrivalCircular)},{Convert.ToChar(ArrivalPerpendicular)},{BOD.ToString("F3")},{DestinationWayPointId:D3},{Bearing.ToString("F3")},{Heading.ToString("F3")}";
      }
    }
  }
}
