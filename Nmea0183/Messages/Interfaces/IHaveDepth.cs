namespace Nmea0183.Messages.Interfaces
{
  public interface IHaveDepth 
  { double Depth { get; }
    double Offset { get; }
  }
}