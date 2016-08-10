using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsoleLog
{
  class Program
  {
    static int Main(string[] args)
    {
      var commandName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);

      if (args.Length != 1)
      {
        Console.Error.WriteLine(@"
  This program writes all messages coming in on an exchange to the console.

  Usage: {0} exchange

  Where:
   exchange is the name of a RabbitMQ exchange.

", commandName);
        Console.ReadKey();
        return -1;
      }

      var exchange = args[0];

      var factory = new ConnectionFactory() {HostName = "localhost"};
      using (var connection = factory.CreateConnection())
      using (var model = connection.CreateModel())
      {
        model.ExchangeDeclare(exchange, "fanout");
        var queueName = model.QueueDeclare().QueueName;
        model.QueueBind(queue: queueName, exchange: exchange, routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(model);
        consumer.Received += (m, ea) =>
        {
          var message = Encoding.UTF8.GetString(ea.Body);
          Console.Error.WriteLine(message);
        };

        model.BasicConsume(queue: queueName, noAck: true, consumer: consumer);

        Console.ReadKey();
        return 0;
      }
    }
  }
}
