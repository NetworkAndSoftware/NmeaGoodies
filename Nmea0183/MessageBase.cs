using System;
using System.Linq;
using System.Reflection.Emit;

namespace Nmea0183
{
  public abstract class MessageBase
  {
    protected MessageBase(string talkerId, string commandName)
    {
      TalkerId = talkerId;
      CommandName = commandName;
    }

    public string TalkerId { get; set; }
    public string CommandName { get; private set; }

    protected abstract string CommandBody { get; }

    private static string Checksum(string s)
    {
      byte sum = s.ToCharArray().Aggregate<char, byte>(0, (current, c) => (byte) (current ^ Convert.ToByte((char) c)));
      return BitConverter.ToString(new[] {sum});
    }

    public override string ToString()
    {
      var line = TalkerId + CommandName + "," + CommandBody;
      return "$" + line + '*' + Checksum(line);
    }
  }
}