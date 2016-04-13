using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nmea0183.Messages;

namespace Nmea0183.Communications
{
  /// <summary>
  ///   Reads and dispatches messages.
  ///   Offers pub/sub
  ///   TODO: This should use an ioc pattern
  /// </summary>
  public class MessageReader
  {
    private const int QUEUE_MAX_LENGTH = 256;

    private static MessageReader _instance;

    public readonly Queue<Tuple<MessageBase, DateTime>> Messages = new Queue<Tuple<MessageBase, DateTime>>(QUEUE_MAX_LENGTH);

    private MessageReader()
    {
      Task.Factory.StartNew(KeepReading);
    }

    public static MessageReader Instance => _instance ?? (_instance = new MessageReader());

    private void KeepReading()
    {
      while (true)
      {
        try
        {
          using (var reader = new StreamReader(Connector.Instance.Stream, Encoding.UTF8))
          {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
              try
              {
                var message = MessageBase.Parse(line);

                lock (Messages)
                {
                  // if Queue is full simply throw away old messages
                  while (Messages.Count >= QUEUE_MAX_LENGTH)
                    Messages.Dequeue();

                  Messages.Enqueue(new Tuple<MessageBase, DateTime>(message, DateTime.UtcNow));
                }
              }
              catch (FormatException x)
              { 
                Trace.WriteLine("Discarding message that can't be parsed. Exception was:");
                Trace.WriteLine(x);
              }
            }
          }
        }
        catch (Exception x)
        {
          var message = "Exception in read loop:" + x + "\n\nResuming...";
          Trace.WriteLine(message);
          Console.WriteLine(message);
        }
      }
    }
  }
}