using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nmea0183.Communications
{
  internal class Connector
  {
    private const int TCP_PORT = 10110;
    private readonly IPAddress _ipAddress = IPAddress.Loopback;
    private readonly int _reconnectdelay = 250; // 250 milliseconds

    private SocketWithStream _socketWithStream;

    private Connector()
    {
      TriggerSocketConnecting();
    }

    static Connector _instance;


    public static Connector Instance => _instance ?? (_instance = new Connector());

    public void SendLine(string line)
    {
      var s = line + "\r\n";

      SocketSendWithReconnect(Encoding.ASCII.GetBytes(s));
    }


    private Task _tryingtoconnect;

    // Connects socket on a background thread. Keeps trying to connect on error, but returns immediately
    private void TriggerSocketConnecting()
    {
      if (_tryingtoconnect != null)
      {
        if (!_tryingtoconnect.IsCompleted && !_tryingtoconnect.IsCanceled)
          return;

        _tryingtoconnect.Dispose();
      }

      _tryingtoconnect = Task.Factory.StartNew(KeepTryingToConnect);
    }

    private void KeepTryingToConnect()
    {
      while (true)
      {
        try
        {
          _socketWithStream = new SocketWithStream(_ipAddress, TCP_PORT);
          return;
        }
        catch (Exception x)
        {
          Trace.WriteLine("Ignoring exception. Waiting and retrying. Exception detail:");
          Trace.WriteLine(x.ToString());
          Thread.Sleep(_reconnectdelay);
        }
      }
    }


    // Send data. If there is a problem, triggering background reconnect and just discard the data
    private void SocketSendWithReconnect(byte[] bytes)
    {
      try
      {
        _socketWithStream._socket.Send(bytes);
      }
      catch (SocketException socketException)
      {
        switch (socketException.SocketErrorCode)
        {
          case SocketError.ConnectionAborted:
          case SocketError.NotConnected:
            TriggerSocketConnecting();
            break;

          default:
            throw;
        }

      }
      catch (NullReferenceException)
      {
        if (null == _socketWithStream)
          TriggerSocketConnecting();
        else
          throw;
      }
    }

    public NetworkStream Stream
    {
      get
      {
        if (null == _socketWithStream)
        {
          TriggerSocketConnecting();
          _tryingtoconnect.Wait();
        }

        return _socketWithStream._Stream;
      }
    }
  }
}


