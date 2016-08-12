using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nmea0183.Messages;
using RabbitMQ.Client;

namespace Nmea0183.Communications
{
  public class MessageSender : IDisposable
  {
    private readonly string _exchange;
    private readonly IConnection _connection;
    private readonly IModel _model;

    public MessageSender(string hostname, string exchange)
    {
      _exchange = exchange;

      var factory = new ConnectionFactory() { HostName = hostname };
      _connection = factory.CreateConnection();
      _model = _connection.CreateModel();
      _model.ExchangeDeclare(exchange, "fanout");

    }

    public void Send(MessageBase message)
    {
      Send(message.ToString());
    }

    public void Send(string line)
    {
      var body = Encoding.UTF8.GetBytes(line);
      // ReSharper disable once AccessToDisposedClosure
      _model.BasicPublish(exchange: _exchange,
        routingKey: string.Empty,
        basicProperties: null,
        body: body);

    }
    // ReSharper disable once FunctionNeverReturns
    public void Dispose()
    {
      _connection.Dispose();
      _model.Dispose();
    }
  }
}

