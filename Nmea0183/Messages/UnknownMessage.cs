
namespace Nmea0183.Messages
{
  /// <summary>
  /// A message class that can be used to construct messages with any type of content
  /// </summary>
  public class UnknownMessage : MessageBase
  {
    public UnknownMessage(string talkerId, string commandName, string commandbody) : base(talkerId)
    {
      CommandBody = commandbody;
      CommandName = commandName;
    }

    public override string CommandName { get; }
    protected override string CommandBody { get; }
  }
}
