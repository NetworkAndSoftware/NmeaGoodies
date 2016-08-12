using System;
using System.Windows.Threading;
using Nmea0183.Messages;

namespace Nmea0183.Communications
{
  public class RepeatingSender 
  {
    private readonly MessageSender _sender;
    private readonly Action<MessageBase> _onBeforeSend;
    private readonly DispatcherTimer _timer = new DispatcherTimer();

    public MessageBase Message { get; set; }

    public RepeatingSender(MessageSender sender, Action<MessageBase> onBeforeSend = null) : this(sender, TimeSpan.FromSeconds(1), onBeforeSend)
    {
    }

    public RepeatingSender(MessageSender sender, TimeSpan interval, Action<MessageBase> onBeforeSend = null)
    {
      _sender = sender;
      _onBeforeSend = onBeforeSend;
      _timer.Tick += TimerOnTick;
      _timer.Interval = interval;
      
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

      _sender.Send(Message);
    }
  }
}