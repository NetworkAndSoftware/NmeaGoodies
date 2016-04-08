
namespace Nmea0183
{
  /// <summary>
  /// A message class that can be used to construct messages with any type of content
  /// </summary>
  public class UnTypedMessage : MessageBase
  {
    private readonly string _commandbody;

    public UnTypedMessage(string talkerId, string commandName, string commandbody) : base(talkerId, commandName)
    {
      _commandbody = commandbody;
    }

    protected override string CommandBody
    {
      get { return _commandbody; } 
    }
  }
}
