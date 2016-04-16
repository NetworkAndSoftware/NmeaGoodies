using Nmea0183.Constants;

namespace Nmea0183.Messages
{
  public interface IHaveHeading
  {
    IMessageCompassValue Heading { get; set; }
  }

  ///<summary>Heading, True. 
  /// See http://www.catb.org/gpsd/NMEA.html#_hdm_heading_magnetic </summary>
  [CommandName(MessageName.HDT)]
  // ReSharper disable once InconsistentNaming
  public class HDT : MessageBase, IHaveHeading
  {
    public IMessageCompassValue Heading { get; set; }

    public HDT(string talkerId) : base(talkerId)
    {
    }

    internal HDT(string talkedId, string[] parts) : base(talkedId)
    {
      Heading = MessageCompassValueFactory.FromMessageParts(parts[0], parts[1]);
    }

    protected override string CommandBody => Heading.ToString("F3");

  }
}