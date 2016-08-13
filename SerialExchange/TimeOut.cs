using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SerialExchange
{
  internal class TimeOut : IDisposable
  {
    private readonly Timer _timer;

    public TimeOut(int seconds)
    {
      _timer = new Timer(1000 * seconds) { AutoReset = false };
      _timer.Elapsed += (sender, args) =>
      {
        Elapsed?.Invoke();
        Reset();
      };
      _timer.Start();
    }

    public delegate void ElapsedEventHandler();

    public event ElapsedEventHandler Elapsed; 

    public void Reset()
    {
      _timer.Stop();
      _timer.Start();
    }

    public void Dispose()
    {
      _timer.Stop();
      _timer.Dispose();
    }
  }
}
