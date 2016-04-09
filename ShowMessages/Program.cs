using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nmea0183;
using Nmea0183.Communications;

namespace ShowMessages
{
  class Program
  {
    static void Main(string[] args)
    {
      //MessageReader.Instance.IncomingMessage += message => Console.WriteLine(message.ToString());

      while (true)
      {
        var messages = MessageReader.Instance.Messages;

        lock (messages)
        {
          while (messages.Any())
          {
            Console.WriteLine(messages.Dequeue().ToString());
          }
        }
        Thread.Sleep(100);

        if (Console.KeyAvailable)
          return;
      }
    }
  }
}
