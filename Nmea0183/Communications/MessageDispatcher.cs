using System.Linq;
using Nmea0183.Messages;

namespace Nmea0183.Communications
{
  public static class MessageDispatcher
  {
    public delegate void IncomingMessageHandler(MessageBase message);
    public static event IncomingMessageHandler IncomingMessage;

    /// <summary>
    /// Checks if there are any messages in the reader and, if yes, triggers the event
    /// </summary>
    public static void Poll()
    {
      var messageReader = MessageReader.Instance;

      lock (messageReader.Messages)
      {
        while (messageReader.Messages.Any())
          IncomingMessage?.Invoke(messageReader.Messages.Dequeue());
      }
    }
  }
}