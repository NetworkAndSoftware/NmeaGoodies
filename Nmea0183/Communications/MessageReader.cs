using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nmea0183.Helpers;
using Nmea0183.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nmea0183.Communications
{
  /// <summary>
  ///   Reads and dispatches messages.
  /// </summary>
  public class MessageReader : IDisposable
  {
    private IConnection _connection;
    private IModel _model;
    private EventingBasicConsumer _consumer;

    public delegate void MessageEventHandler(MessageBase message, DateTime datetime);

    public event MessageEventHandler Message;

    private void OnMessage(MessageBase message, DateTime datetime)
    {
      Message?.Invoke(message, datetime);
    }

    public MessageReader(string hostname, string exchange)
    {
      var factory = new ConnectionFactory() {HostName = hostname};
      _connection = factory.CreateConnection();
      _model = _connection.CreateModel();

      _model.ExchangeDeclare(exchange, "fanout");

      var queueName = _model.QueueDeclare().QueueName;
      _model.QueueBind(queue: queueName, exchange: exchange, routingKey: string.Empty);

      _consumer = new EventingBasicConsumer(_model);

      _consumer.Received += (m, ea) =>
      {
        var line = Encoding.UTF8.GetString(ea.Body);
        try
        {
          var message = MessageBase.Parse(line);
          OnMessage(message, ea.BasicProperties.Timestamp.DateTime());
        }
        catch (FormatException x)
        {
          Trace.WriteLine("Discarding message that can't be parsed. Exception was:");
          Trace.WriteLine(x);
        }
      };

      _model.BasicConsume(queue: queueName, noAck: true, consumer: _consumer);
    }

    public void Dispose()
    {
      _model.Dispose();
      _connection.Dispose();
    }
  }
}