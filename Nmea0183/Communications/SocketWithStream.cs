using System.Net;
using System.Net.Sockets;

namespace Nmea0183.Communications
{
  internal class SocketWithStream
  {
    public readonly Socket _socket;
    public readonly NetworkStream _Stream;

    public SocketWithStream(IPAddress address, int port)
    {
      _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      _socket.Connect(address, port);
      _Stream = new NetworkStream(_socket);
    }
  }
}
