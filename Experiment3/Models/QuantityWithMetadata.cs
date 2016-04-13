using System;

namespace Experiment3.Models
{
  internal interface IQuantityWithMetaData
  {
    bool IsStale { get; }
    DateTime? Updated { get; }
    bool UIToldItsStale { get; set; }

  }

  public enum SourceType
  {
    Unknown = 0,
    External,
    Calculated,
    User
  };


  /// <summary>
  /// TODO: expiration time probably shouldn't be hardcoded, and probably shouldn't live here.
  /// </summary>
  internal class QuantityWithMetadataBase
  {
    protected static readonly TimeSpan Expiration = TimeSpan.FromSeconds(3);
  }

  internal class QuantityWithMetadata<T> : QuantityWithMetadataBase, IQuantityWithMetaData
  {
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
    
    public SourceType Source { get; set; }

    public bool IsStale => !Updated.HasValue || DateTime.UtcNow - Updated > Expiration;

    public DateTime? Updated { get; private set; }

    // TODO: this shouldn't be here. It has nothing to do with the data itself. Move back to viewmodel
    public bool UIToldItsStale { get; set; }
    

    public static implicit operator QuantityWithMetadata<T>(T value)
    {
      return new QuantityWithMetadata<T>(value);
    }

    public static implicit operator T(QuantityWithMetadata<T> data)
    {
      return data.Value;
    }

    public QuantityWithMetadata(T value)
    {
      Value = value;
    }
  }
}