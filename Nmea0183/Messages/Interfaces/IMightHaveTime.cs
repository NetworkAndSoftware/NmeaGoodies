using System;

namespace Nmea0183.Messages.Interfaces
{
  public interface IMightHaveTime
  {
    TimeSpan? Time { get; set; }
  }
}