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
  /// Reads and dispatches messages. 
  /// 
  /// Offers pub/sub
  /// 
  /// TODO: This should use an ioc pattern
  /// </summary>
  public class MessageReader
  {
    private const int QUEUE_MAX_LENGTH = 256;
    
    public readonly Queue<MessageBase> Messages = new Queue<MessageBase>(QUEUE_MAX_LENGTH);

    static MessageReader _instance;

    public static MessageReader Instance => _instance ?? (_instance = new MessageReader());

    private MessageReader()
    {
      Task.Factory.StartNew(KeepReading);
    }

    private void KeepReading()
    {
      using (StreamReader reader = new StreamReader(Connector.Instance.Stream, Encoding.UTF8))
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

              Messages.Enqueue(message);
            }

          }
          catch (FormatException x)
          { Trace.WriteLine("Discarding message that can't be parse. Exception was:");
            Trace.WriteLine(x);
          }
        }
      }
    }

  }
}
