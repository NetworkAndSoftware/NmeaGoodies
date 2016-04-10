using Nmea0183.Messages.Enum;

namespace Nmea0183.Messages.Interfaces
{
  public interface IHasStatus
  {
    Flag Status { get; set; }
  }
}