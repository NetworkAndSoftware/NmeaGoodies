using System;

namespace Experiment3.Models
{
  internal interface IQuantityWithMetaData
  {
    bool IsStale { get; }
    DateTime? Updated { get; }
    bool UIToldItsStale { get; set; }
  }

  internal class QuantityWithMetadata<T> : IQuantityWithMetaData
  {
    private static readonly TimeSpan Expiration = TimeSpan.FromSeconds(3);

    private T _value;

    public T Value
    {
      get { return _value; }
      set
      {
        _value = value;
        Updated = DateTime.UtcNow;
      }
    }

    public enum SourceType
    { Unknown = 0,
      External,
      Calculated,
      User
    };

    public SourceType Source { get; set; }

    public bool IsStale => !Updated.HasValue || DateTime.UtcNow - Updated > Expiration;

    public DateTime? Updated { get; private set; }

    // TODO: this shouldn't be here. It has nothing to do with the data itself. Move back to viewmodel
    public bool UIToldItsStale { get; set; }



    public static implicit operator QuantityWithMetadata<T>(T value)
    {
      return new QuantityWithMetadata<T>() { Value = value };
    }

    public static implicit operator T(QuantityWithMetadata<T> data)
    {
      return data.Value;
    }
  }
}