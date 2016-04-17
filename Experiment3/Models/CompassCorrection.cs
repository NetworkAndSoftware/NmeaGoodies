using System;
using System.Windows;
using Newtonsoft.Json;
using Nmea0183;

namespace Experiment3.Models
{ /// <summary>
  /// This class tries to establish a compass correction, to go from measured compass value to real heading
  /// 
  /// It does this by tracking a running mean between compass course and course over ground. The thinking is, that over a long period of time, the influence of currents, wind etc will average out.
  /// 
  /// This correction might be different for different compass courses. Therefore we create a number of segments and get the mean for each segment.
  /// </summary>
  internal class CompassCorrection
  {
    private readonly MagneticContext _context;

    public struct RunningMean
    {
      /// <summary>
      /// Segment start i.g. 0 means the 0-1 degree interval
      /// </summary>
      public int S { get; set; }
      /// <summary>
      /// The running mean for this segment
      /// </summary>
      public decimal M { get; set; }
      /// <summary>
      /// The number of samples used for this segment
      /// </summary>
      public ulong C { get; set; }
    }

    public RunningMean[] MeansByMeasuredDegree { get; set; }

    public CompassCorrection(MagneticContext context)
    {
      _context = context;
      MeansByMeasuredDegree = new RunningMean[360];
      for (var i = 0; i < 360; i++)
        MeansByMeasuredDegree[i] = new RunningMean() { S = i };
    }

    public void AddSample(IMessageCompassValue heading, IMessageCompassValue cog)
    {
      var segment = (int)heading.Value;
      var deviation = (decimal)_context.ExecuteFunction(heading, cog, (value1, value2) => value1 - value2);

      MeansByMeasuredDegree[segment].M = (deviation + MeansByMeasuredDegree[segment].C * MeansByMeasuredDegree[segment].M) / (MeansByMeasuredDegree[segment].C + 1);
      MeansByMeasuredDegree[segment].C++;
    }

    [JsonIgnore]
    public decimal Mean
    {
      get
      {
        decimal total = 0;
        int count = 0;

        for (var i = 0; i < 360; i++)
        {
          if (MeansByMeasuredDegree[i].C > 0)
          {
            total += MeansByMeasuredDegree[i].M;
            count++;
          }
        }

        return count > 0 ? total / count : 0;
      }
    }

    [JsonIgnore]
    public ulong Count
    {
      get
      {
        ulong count = 0;

        for (var i = 0; i < 360; i++)
          count += MeansByMeasuredDegree[i].C;

        return count;
      }
    }


    public IMessageCompassValue CorrectHeading(IMessageCompassValue heading)
    {
      var segment = (int)heading.Value;

      return (_context.ExecuteFunction(heading, h => h - (double)MeansByMeasuredDegree[segment].M));
    }
  }
}