using System;
using System.Windows.Threading;

namespace Nmea0183.Communications
{
  /// <summary>
  /// Have the MessageDispatcher poll periodically
  /// </summary>
  public static class WpfMessagePoller
  {
    private static readonly DispatcherTimer Timer;

    /// <summary>
    /// Sets the polling interval 
    /// </summary>
    public static void SetInterval(int interval)  
    { Timer.Stop();
      Timer.Interval = TimeSpan.FromMilliseconds(interval);
      Timer.Start();
    }

    static WpfMessagePoller()
    {
      Timer = new DispatcherTimer();
      Timer.Tick += (o, e) => MessageDispatcher.Poll();
    }
  }
}