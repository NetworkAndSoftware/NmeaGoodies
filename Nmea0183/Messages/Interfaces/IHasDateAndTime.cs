using System;

namespace Nmea0183.Messages.Interfaces
{
  public interface IHasDateAndTime
  {
    DateTime? DateTime { get; set; }
  }
}