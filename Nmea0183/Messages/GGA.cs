using Nmea0183.Constants;

namespace Nmea0183.Messages
{
  ///<summary>Fix information</summary>
  [CommandName(MessageName.GGA)]
  // ReSharper disable once InconsistentNaming
  public class GGA : MessageBase
  {
    public GGA(string talkerId) : base(talkerId)
    {
    }

    internal GGA(string talkerId, string[] bodyparts) : base(talkerId)
    {
      throw new System.NotImplementedException();
    }

    protected override string CommandBody { get; }
  }
}
