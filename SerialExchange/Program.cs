using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SerialExchange
{
  class Program
  {
    static int Main(string[] args)
    { var commandName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);

      if (args.Length < 3)
      { Console.Error.WriteLine(@"
  Usage: {0} COMx receiveexchange sendqueue

  Where:
   COMx is the name of a serial port, sendqueue is the name of a RabbitMQ
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


      SerialPort port;

      try
      {
        port = new SerialPort(namecomport);
        port.Open();
      }
      catch (Exception x)
      {
        Console.Error.WriteLine(x.ToString());
        return -1;
      }

      var factory = new ConnectionFactory() { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      using (var model = connection.CreateModel())
      {
        model.ExchangeDeclare(nameReceiveExchange, "fanout");
        model.ExchangeDeclare(nameSendExchange, "fanout");

        {
          var queueName = model.QueueDeclare().QueueName;
          model.QueueBind(queue: queueName,
            exchange: nameSendExchange,
            routingKey: string.Empty);

          var consumer = new EventingBasicConsumer(model);
          consumer.Received += (m, ea) =>
          {
            var message = Encoding.UTF8.GetString(ea.Body);
            Console.Error.WriteLine(message);
          };

          model.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
        }

        var stop = false;

        var task = Task.Factory.StartNew(() =>
        {
          // ReSharper disable once AccessToModifiedClosure
          while (!stop)
          {
            var line = port.ReadLine();

            var body = Encoding.UTF8.GetBytes(line);
            // ReSharper disable once AccessToDisposedClosure
            model.BasicPublish(exchange: nameReceiveExchange,
              routingKey: String.Empty,
              basicProperties: null,
              body: body);

          }
          // ReSharper disable once FunctionNeverReturns
        });

        Console.WriteLine("{0} active.\nSerial port '{1}' -> exchange '{2}'.\nExchange '{3}' -> serial port '{1}'\nPress any key to exit.", commandName, namecomport, nameReceiveExchange, nameSendExchange);
        Console.ReadKey();
        stop = true;
        task.Wait();
      }

      return 0;
    }
  }
}
