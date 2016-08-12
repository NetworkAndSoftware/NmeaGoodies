using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Nmea0183.Constants;

namespace Nmea0183.Messages
{
  [CommandName(MessageName.PSMDOV)]
  public class PSMDOV : MessageBase
  {
    private readonly ushort _bits;

    public PSMDOV(string talkerId) : base(talkerId)
    {
    }

    internal PSMDOV(string talkedId, string[] parts) : base(talkedId)
    {
      _bits = ushort.Parse(parts[0]);
    }

    protected override string CommandBody => _bits.ToString();
  }
}
