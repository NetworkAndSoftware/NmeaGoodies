using System.Diagnostics;
using System.IO;
using System.Windows.Markup;
using Newtonsoft.Json;

namespace Experiment3.Models
{
  internal class CompassCorrectionPersister
  {
    private readonly string _filename;

    public CompassCorrectionPersister(string filename)
    {
      _filename = filename;
    }

    public void Read(CompassCorrection correction)
    {
      using (var stream = File.Open(_filename, FileMode.OpenOrCreate))
      using (var reader = new StreamReader(stream))
      using (var jsonTextReader = new JsonTextReader(reader))
      {
        var serializer = new JsonSerializer();
        serializer.Populate(jsonTextReader, correction);
      }
    }

    public void Write(CompassCorrection correction)
    {
      try
      {
        using (var stream = File.Open(_filename, FileMode.OpenOrCreate))
        using (var writer = new StreamWriter(stream))
        using (JsonWriter jsonWriter = new JsonTextWriter(writer) {Formatting = Formatting.Indented})
        {
          var serializer = new JsonSerializer();
          serializer.Serialize(jsonWriter, correction);
        }
      }
      catch (IOException x)
      { Trace.WriteLine("Ignoring exception while trying to write compass correction: " + x);
      }
    }
  }
}