using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nmea0183.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

namespace SerialExchange
{
  class Program
  {
    static int Main(string[] args)
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

      using (var port = new SerialPort(namecomport))
      {
        try
        {

          port.Open();
        }
        catch (Exception x)
        { Trace.WriteLine(x.ToString());
          Console.Error.WriteLine(x.ToString());
          return -1;
        }

        var factory = new ConnectionFactory() {HostName = "localhost"};
        using (var connection = factory.CreateConnection())
        using (var model = connection.CreateModel())
        {
          model.ExchangeDeclare(nameReceiveExchange, "fanout");
          model.ExchangeDeclare(nameSendExchange, "fanout");

          {
            var queueName = model.QueueDeclare().QueueName;
            model.QueueBind(queue: queueName,
              exchange: nameReceiveExchange,
              routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (m, ea) =>
            {
              var message = Encoding.UTF8.GetString(ea.Body);
              port.WriteLine(message);
            };

            model.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
          }

          var stop = false;

          using (var task = Task.Factory.StartNew(() =>
          {
            // ReSharper disable once AccessToModifiedClosure
            while (!stop)
            {
              var line = port.ReadLine();

              var body = Encoding.UTF8.GetBytes(line);
              // ReSharper disable once AccessToDisposedClosure
              var basicProperties = new BasicProperties() {Timestamp = DateTime.Now.AmqpTimestamp()};
              model.BasicPublish(exchange: nameReceiveExchange,
                routingKey: String.Empty,
                basicProperties: basicProperties,
                body: body);

            }
            // ReSharper disable once FunctionNeverReturns
          }))
          {

            Console.WriteLine(
              "{0} active.\nSerial port '{1}' -> exchange '{2}'.\nExchange '{3}' -> serial port '{1}'\nPress any key to exit.",
              commandName, namecomport, nameReceiveExchange, nameSendExchange);
            Console.ReadKey();
            stop = true;
          }
        }

        return 0;
      }
    }
  }
}