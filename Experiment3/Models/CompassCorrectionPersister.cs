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

    private static int nested = 0;
    public void Write(CompassCorrection correction)
    {
      nested ++;
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
      {
        x.Data.Add("nested", nested);
        throw;
      }
      finally
      {
        nested--;
      }
    }
  }
}