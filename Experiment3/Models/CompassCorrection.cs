using Experiment3.Annotations;
using Newtonsoft.Json;
using Nmea0183;

namespace Experiment3.Models
{
  internal class CompassCorrection
  {
    private readonly MagneticContext _context;
    public double Mean { get; set; }
    public ulong Count { get; set; }

    public CompassCorrection(MagneticContext context, double mean = 0, ulong count = 0)
    {
      _context = context;
      Count = count;
      Mean = mean;
    }

    public void AddSample(IMessageCompassValue heading, IMessageCompassValue cog)
    {
      var deviation = _context.ExecuteFunction(heading, cog, (value1, value2) => value1 - value2);

      Mean = (deviation + Count * Mean) / (Count + 1);

      Count++;
    }

    public IMessageCompassValue CorrectHeading(IMessageCompassValue value)
    {
      return (_context.ExecuteFunction(value, v => v - Mean));
    }
  }
}