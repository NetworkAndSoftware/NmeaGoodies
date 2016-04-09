using System;
using Nmea0183.Constants;

namespace Nmea0183
{
  public class CommandNameAttribute : Attribute
  {
    public CommandNameAttribute(MessageNames name)
    {
      Name = name;
    }

    public MessageNames Name { get; private set; }

  }
}