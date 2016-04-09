using System;
using System.Windows.Threading;

namespace Nmea0183.Communications
{
  public static class WpfMessagePoller
  {
    private static readonly DispatcherTimer Timer;

    /// <summary>
    /// Reference this dummy method to cause the static class to be created
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