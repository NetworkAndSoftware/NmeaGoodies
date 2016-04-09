using System;
using Nmea0183.Constants;

namespace Nmea0183
{
  public class CommandNameAttribute : Attribute
  {
    public CommandNameAttribute(MessageName name)
    {
      Name = name;
    }

    public MessageName Name { get; private set; }

  }
}