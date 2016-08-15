using System;
using System.IO;
using System.Text;

namespace TcpExchange
{
  internal class ContinuousStringReader
  {
    public delegate void ReceivedHandler(string s);

    private readonly byte[] _readbuffer = new byte[4096];
    private readonly Stream _stream;
    private bool _stop;

    public ContinuousStringReader(Stream stream)
    {
      _stream = stream;
    }

    public void Start()
    {
      _stop = false;
      var ar = BeginRead();
    }

    public void Stop()
    {
      _stop = true;
    }

    private IAsyncResult BeginRead()
    {
      try
      {
        return _stream.BeginRead(_readbuffer, 0, _readbuffer.Length, Callback, null);
      }
      catch (Exception x)
      {
        Error?.Invoke(x);
        Stop();
        return null; // an exception ends the reading
      }
    }

    private void Callback(IAsyncResult ar)
    {
      {
        var bytesread = _stream.EndRead(ar);

        if (_stop)
          return;

        var s = Encoding.UTF8.GetString(_readbuffer, 0, bytesread);

        Received?.Invoke(s);

        BeginRead();
      }
    }

    public event ReceivedHandler Received;
    public event Action<Exception> Error;
  }
}