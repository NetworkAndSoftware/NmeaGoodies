using System;
using RabbitMQ.Client;

namespace Nmea0183.Helpers
{
  public static class AmqpTimestampExtensions
  {
    private static readonly DateTime UnixEpoch =
      new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static DateTime FromUNIX(long unixtime)
    {
      return UnixEpoch.AddSeconds(unixtime);
    }

    private static long ToUNIX(DateTime datetime)
    {
      return (long) (datetime.ToUniversalTime() - UnixEpoch).TotalSeconds;
    }

    public static AmqpTimestamp AmqpTimestamp(this DateTime datetime)
    {
      return new AmqpTimestamp(ToUNIX(datetime));
    }

    public static DateTime DateTime(this AmqpTimestamp amqptimestamp)
    {
      return FromUNIX(amqptimestamp.UnixTime);
    }
  }
}