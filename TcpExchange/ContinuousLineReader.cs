using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpExchange
{
  internal class ContinuousLineReader : ContinuousStringReader
  {
    private readonly StringBuilder _linebuffer = new StringBuilder(1024);

    public ContinuousLineReader(Stream stream) : base(stream)
    {
      base.Received += s =>
      {
        var lines = s.Split('\n').ToList();

        while (lines.Any())
        {
          _linebuffer.Append(lines[0]);
          lines.RemoveAt(0);
          if (!lines.Any())
            continue;
          var line = _linebuffer.ToString().Trim();
          _linebuffer.Clear();

          ReceivedLine?.Invoke(line);
        }
      };
    }

    public event ReceivedHandler ReceivedLine;
  }
}
