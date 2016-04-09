using System;
using System.Windows.Threading;
using Nmea0183.Messages;

namespace Nmea0183.Communications
{
  public class RepeatingSender 
  {
    private readonly Action<MessageBase> _onBeforeSend;
    private readonly DispatcherTimer _timer = new DispatcherTimer();
    
    public MessageBase Message { get; set; }

    public RepeatingSender(int interval = 1, Action<MessageBase> onBeforeSend = null)
    {
      _onBeforeSend = onBeforeSend;
      _timer.Tick += TimerOnTick;
      _timer.Interval = TimeSpan.FromSeconds(interval);
      
    }

    public void Start()
    {
      _timer.Start();
    }

    public void Stop()
    {
      _timer.Stop();
    }

    private void TimerOnTick(object sender, EventArgs eventArgs)
    {
      if (null == Message) 
        return;

      _onBeforeSend?.Invoke(Message);

      Send(Message);
    }

    private void Send(MessageBase apb)
    {
      Connector.Instance.SendLine(apb.ToString());
    }

  }
}