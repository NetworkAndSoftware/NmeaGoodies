using Nmea0183.Constants;

namespace Nmea0183.Messages
{
  /// <summary>
  /// GNSS satellite fault detection (RAIM support) 
  /// (according to http://www.trimble.com/OEM_ReceiverHelp/V4.44/en/NMEA-0183messages_GBS.html )
  /// </summary>
  [CommandName(MessageName.GBS)]
  // ReSharper disable once InconsistentNaming
  public class GBS : MessageBase
  {
    public GBS(string talkerId) : base(talkerId)
    {
    }

    internal GBS(string talkerId, string[] bodyparts) : base(talkerId)
    {
      throw new System.NotImplementedException();
    }

    protected override string CommandBody { get; }
  }
}
