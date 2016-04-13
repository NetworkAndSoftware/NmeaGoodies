using Nmea0183.Constants;
using Nmea0183.Messages.Enum;

namespace Nmea0183.Messages
{
  ///<summary>This is what the Sitex SP110 / AP47 puts out from its compass</summary>
  [CommandName(MessageName.HDM)]
  // ReSharper disable once InconsistentNaming
  public class HDM : MessageBase
  {
    public IMessageCompassValue Heading { get; set; }

    public HDM(string talkerId) : base(talkerId)
    {
    }

    internal HDM(string talkedId, string[] parts) : base(talkedId)
    {
      Heading = MessageCompassValueFactory.FromMessageParts(parts[0], parts[1]);
    }

    protected override string CommandBody => Heading.ToString("F3");

  }
}

