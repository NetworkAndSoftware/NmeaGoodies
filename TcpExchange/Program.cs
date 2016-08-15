using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nmea0183.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

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
      catch (ArgumentException x)
      {
        Console.Error.WriteLine(@"
  Usage: {0} listen|connect address:port receiveexchange sendexchange

  Where:
   listen creates a listening socket or connect attempts to connect
   address:port is the address and port number to listen on or connect to.
   sendexchange is a RabbitMQ queue where data coming in from the TCP
   connection is deposited and receivequeue is the name of a queue where, if
   any messages are placed in it, they will be written to the TCP connection.

", commandName);
        Console.ReadKey();
        return -1;
      }

      var factory = new ConnectionFactory { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      using (var model = connection.CreateModel())
      {
        model.ExchangeDeclare(configuration.ReceiveExchange, "fanout");
        model.ExchangeDeclare(configuration.SendExchange, "fanout");

        if (configuration.Listen)
        {
          var listeningsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          listeningsocket.Bind(configuration.EndPoint);
          listeningsocket.Listen(2);
          while (true)
          {
            var connectedsocket = listeningsocket.Accept();
            HandleConnection(connectedsocket, model, configuration.ReceiveExchange, configuration.SendExchange, new ManualResetEventSlim());
          }
        }
        else
        if (configuration.Connect)
        {
          var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          while (true)
          {
            socket.Connect(configuration.EndPoint);
            HandleConnection(socket, model, configuration.ReceiveExchange, configuration.SendExchange, new ManualResetEventSlim());
          }
        }
      }
      return 0;
    }

    private static void HandleConnection(Socket socket, IModel model, string receiveexchange, string sendexchange, ManualResetEventSlim stopevent)
    {
      var stream = new NetworkStream(socket);
      var writer = new StreamWriter(stream);
      var reader = new StreamReader(stream);

      #region Write to socket
      var queueName = model.QueueDeclare().QueueName;
      model.QueueBind(queueName, sendexchange, string.Empty);

      var consumer = new EventingBasicConsumer(model);

      consumer.Received += (m, ea) =>
      {
        var message = Encoding.UTF8.GetString(ea.Body);
        try
        {
          writer.WriteLine(message);
        }
        catch (IOException x)
        {
          var exception = x.InnerException as SocketException;
          if (exception != null && exception.SocketErrorCode == SocketError.ConnectionAborted)
            stopevent.Set();
          else
            throw;
        }

      };

      model.BasicConsume(queueName, true, consumer);

      #endregion
      #region Read from socket

      var continousreader = new ContinuousLineReader(stream);
      continousreader.ReceivedLine += line =>
      {
        var body = Encoding.UTF8.GetBytes(line);

        var basicProperties = new BasicProperties { Timestamp = DateTime.Now.AmqpTimestamp() };
        model.BasicPublish(receiveexchange, string.Empty, basicProperties, body);
      };

      continousreader.Error += exception =>
      {
        stopevent.Set();
      };

      continousreader.Start();
      #endregion

      stopevent.Wait();
    }

    private class Configuration
    {
      public readonly IPEndPoint EndPoint;
      public readonly bool Connect;
      public readonly bool Listen;
      public readonly string ReceiveExchange;
      public readonly string SendExchange;

      public Configuration(string[] args)
      {
        if (args.Length != 4) throw new ArgumentException("Command line Syntax");
        Listen = args[0] == "listen";
        Connect = args[0] == "connect";

        if (!Listen && !Connect)
          throw new ArgumentException("Command line Syntax");

        var s = args[1].Split(':');

        IPAddress ipAddress;

        if (s[0] == "0.0.0.0")
          ipAddress = IPAddress.Any;
        else
        {
          var addresses = Dns.GetHostAddresses(s[0]);
          if (addresses.Length < 1)
            throw new ArgumentException($"unable to resolve \"{s[0]}\".");

          ipAddress = addresses[0];
        }
        EndPoint = new IPEndPoint(ipAddress, int.Parse(s[1]));

        ReceiveExchange = args[2];
        SendExchange = args[3];
      }
    }
  }
}