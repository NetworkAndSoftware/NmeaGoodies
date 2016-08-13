using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nmea0183.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

namespace SerialExchange
{
  internal class Program
  {
    private static int Main(string[] args)
    {
      var commandName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);

      if (args.Length != 3)
      {
        Console.Error.WriteLine(@"
  Usage: {0} COMx receiveexchange sendexchange

  Where:
   COMx is the name of a serial port, sendexchange is the name of a RabbitMQ
   queue where data coming in from the serial port is deposited and
   receivequeue is the name of a queue where, if any messages are placed in
   it, they will be written to the serial port.

", commandName);
        Console.ReadKey();
        return -1;
      }

      var namecomport = args[0];
      var nameReceiveExchange = args[1];
      var nameSendExchange = args[2];

      var stopevent = new ManualResetEventSlim();
      var task = RunSerialExchanger(namecomport, 115200, nameReceiveExchange, nameSendExchange, stopevent, reopenporttimeout: 10, onException: exception =>
        {
          var message = exception.ToString();
          Trace.WriteLine(message);
          Console.Error.WriteLine(message);
        }
      );
      Console.WriteLine(
        "{0} active.\nSerial port '{1}' -> exchange '{2}'.\nExchange '{3}' -> serial port '{1}'\nPress Q to exit.",
        commandName, namecomport, nameReceiveExchange, nameSendExchange);

      while (Console.ReadKey().KeyChar.ToString().ToLower() != "q")
        ;
      stopevent.Set();
      task.Wait();
      return 0;
    }

    private static Task RunSerialExchanger(string namecomport, int baudrate, string nameReceiveExchange,
      string nameSendExchange, ManualResetEventSlim stopevent, Action<Exception> onException = null, int reopenporttimeout = 0)
    {
      var task = Task.Factory.StartNew(() =>
      {
        try
        {
          var factory = new ConnectionFactory { HostName = "localhost" };
          using (var connection = factory.CreateConnection())
          using (var model = connection.CreateModel())
          {
            model.ExchangeDeclare(nameReceiveExchange, "fanout");
            model.ExchangeDeclare(nameSendExchange, "fanout");

            // port will be recreated each time we time out
            using (var timeout = new TimeOut(reopenporttimeout))
            {
              Func<SerialPort> createport = () =>
              {
                var p = new SerialPort(namecomport, baudrate);
                p.Open();
                return p;
              };

              var port = createport();
              timeout.Elapsed += () =>
              { port.Close();
                port.Dispose();
                port = createport();
              };

              #region Write to serial port

              var queueName = model.QueueDeclare().QueueName;
              model.QueueBind(queueName, nameReceiveExchange, string.Empty);

              var consumer = new EventingBasicConsumer(model);
              consumer.Received += (m, ea) =>
              {
                var message = Encoding.UTF8.GetString(ea.Body);
                port.WriteLine(message);
              };

              model.BasicConsume(queueName, true, consumer);

              #endregion

              #region Receive from serial port

              var linebuffer = new StringBuilder(1024);

              port.ErrorReceived += (sender, args) =>
              {
                var error = $"Error received: {args.ToString()}";
                Console.Error.WriteLine(error);
                Trace.WriteLine(error);
              };

              port.DataReceived += (sender, eventArgs) =>
              {
                timeout?.Reset();

                var data = port.ReadExisting();

                var lines = data.Split('\n').ToList();

                while (lines.Any())
                {
                  linebuffer.Append(lines[0]);
                  lines.RemoveAt(0);
                  if (!lines.Any())
                    continue;
                  var line = linebuffer.ToString().Trim();

                  var body = Encoding.UTF8.GetBytes(line);

                  var basicProperties = new BasicProperties {Timestamp = DateTime.Now.AmqpTimestamp()};
                  model.BasicPublish(nameReceiveExchange, string.Empty, basicProperties, body);

                  linebuffer.Clear();
                }
              };

              #endregion

              stopevent.Wait();
              port.Close();
              port.Dispose();
            }
          }
        }
        catch (Exception x)
        {
          if (onException != null)
            onException(x);
          else
            throw;
        }
      });


      return task;
    }
  }
}
