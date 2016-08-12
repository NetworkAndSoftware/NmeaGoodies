using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nmea0183.Constants;
using Nmea0183.Messages.Interfaces;

namespace Nmea0183.Messages
{
  [CommandName(MessageName.DPT)]
  // ReSharper disable once InconsistentNaming
  public class DPT : MessageBase, IHaveDepth
  {
    public DPT(string talkerId) : base(talkerId)
    {
    }

    internal DPT(string talkedId, string[] parts) : base(talkedId)
    {
      Depth = double.Parse(parts[0]);
      Offset = double.Parse(parts[1]);
    }

    protected override string CommandBody => $"{Depth:F1},{Offset:F1}";

    public double Depth { get; }
    public double Offset { get; }
  }
}
