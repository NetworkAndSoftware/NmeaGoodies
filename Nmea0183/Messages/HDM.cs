using System;
using Nmea0183.Constants;

namespace Nmea0183.Messages
{
  ///<summary>This is what the Sitex SP110 / AP47 puts out from its compass</summary>
  [CommandName(MessageName.HDM)]
  // ReSharper disable once InconsistentNaming
  public class HDM : MessageBase
  {
    public double Heading { get; set; }
    public MagneticOrTrue Type { get; set; }

    public HDM(string talkerId) : base(talkerId)
    {
    }

    internal HDM(string talkedId, string[] parts) : base(talkedId)
    {
      Heading = double.Parse(parts[0]);
      Type =  ParseOneLetterEnumByValue<MagneticOrTrue>(parts[1]);
    }

    protected override string CommandBody => $"{Heading:F3},{F(Type)}";

  }
}

