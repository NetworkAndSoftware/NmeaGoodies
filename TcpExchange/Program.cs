using System;
using System.IO;
using System.Reflection;

namespace TcpExchange
{
  internal class Program
  {
    private static int Main(string[] args)
    {
      var commandName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);

      Configuration configuration;

      try
      {
        configuration = new Configuration(args);
      }
      catch (ArgumentException)
      {
        Console.Error.WriteLine(@"
  Usage: {0} -l|-c address:port receiveexchange sendexchange

  Where:
   -l creates a listening socket or -c attempts to connect
   address:port is the address and port number to listen on or connect to.
   sendexchange is a RabbitMQ queue where data coming in from the TCP
   connection is deposited and receivequeue is the name of a queue where, if
   any messages are placed in it, they will be written to the TCP connection.

   When listening, only one concurrent connection will be accepted.

", commandName);
        Console.ReadKey();
        return -1;
      }



      return 0;
    }

    private class Configuration
    {
      public readonly string Address;
      public readonly bool Connect;
      public readonly bool Listen;
      public readonly string Port;
      public readonly string ReceiveExchange;
      public readonly string SendExchange;

      public Configuration(string[] args)
      {
        if (args.Length == 4)
        {
          Listen = args[0] == "-l";
          Connect = args[0] == "-c";

          if (Listen || Connect)
          {
            var s = args[1].Split(':');
            Address = s[0];
            Port = s[1];
            if (!string.IsNullOrWhiteSpace(Address) && !string.IsNullOrWhiteSpace(Port))
            {
              ReceiveExchange = args[2];
              SendExchange = args[3];
              return;
            }
          }
        }
        throw new ArgumentException("Command line Syntax");
      }
    }
  }
}