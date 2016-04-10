using Nmea0183.Constants;

namespace Nmea0183.Messages
{
  ///<summary>Lat/Lon data</summary>
  [CommandName(MessageName.GLL)]
  // ReSharper disable once InconsistentNaming
  public class GLL : MessageBase
  {
    public GLL(string talkerId) : base(talkerId)
    {
    }

    internal GLL(string talkerId, string[] bodyparts) : base(talkerId)
    {
      throw new System.NotImplementedException();
    }

    protected override string CommandBody { get; }
  }
}
