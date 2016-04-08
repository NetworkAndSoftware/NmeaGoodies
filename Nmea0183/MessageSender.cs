using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Threading;

namespace Nmea0183
{
  public class MessageSender 
  {
    private readonly Action<MessageBase> _onBeforeSend;
    private readonly DispatcherTimer _timer = new DispatcherTimer();
    private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public MessageBase Message { get; set; }

    public MessageSender(IPAddress ipAddress, int port = 10110, int interval = 1, Action<MessageBase> onBeforeSend = null)
    {
      _onBeforeSend = onBeforeSend;
      SocketConnect(port, ipAddress);
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

    private void SocketConnect(int port, IPAddress address)
    {
      _socket.Connect(address, port);
    }

    private void TimerOnTick(object sender, EventArgs eventArgs)
    {
      if (null == Message) 
        return;

      if (null != _onBeforeSend)
        _onBeforeSend(Message);

      Send(Message);
    }

    private void Send(MessageBase apb)
    {
      SendLine(apb.ToString());
    }

    private void SendLine(string line)
    {
      var s = line + "\r\n";

      SocketSendWithReconnect(Encoding.ASCII.GetBytes(s));
    }

    private void SocketSendWithReconnect(byte[] bytes)
    {
      try
      {
        _socket.Send(bytes);
      }
      catch (SocketException socketException)
      {
        switch (socketException.SocketErrorCode)
        {
          case SocketError.ConnectionAborted:
            SocketConnect(10110, IPAddress.Loopback);
            break;
          default:
            throw;
        }

        // try one more time
        _socket.Send(bytes);
      }
    }

  }
}