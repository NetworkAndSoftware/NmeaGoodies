using System;

namespace Geometry
{
  public class Length : IEquatable<Length>
  {
    private const double METERSPERMILE = 1609.344;
    private const double METERSPERNAUTICALMILE = 1852;

    public static readonly Length Zero = new Length(0);

    private readonly double _meters;

    private Length(double meters)
    {
      _meters = meters;
    }

    public bool Equals(Length other)
    {
      return this == other;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((Length) obj);
    }

    public override int GetHashCode()
    {
      return _meters.GetHashCode();
    }

    public static Length FromMeters(double meters)
    {
      return new Length(meters);
    }

    public static Length FromMiles(double miles)
    {
      return new Length(miles*METERSPERMILE);
    }

    public static Length FromNauticalMiles(double miles)
    {
      return new Length(miles*METERSPERNAUTICALMILE);
    }

    public double Meters()
    {
      return _meters;
    }

    public double Miles()
    {
      return _meters/METERSPERMILE;
    }

    public double NauticalMiles()
    {
      return _meters/METERSPERNAUTICALMILE;
    }

    public static Length operator *(double d, Length length)
    {
      return new Length(d*length._meters);
    }

    public static Length operator *(Length length, double d)
    {
      return d*length;
    }

    public static bool operator >(Length length1, Length length2)
    {
      return length1._meters > length2._meters;
    }

    public static bool operator <(Length length1, Length length2)
    {
      return length2 > length1;
    }

    public static Length operator /(Length length, double d)
    {
      return new Length(length._meters/d);
    }

    public static double operator /(Length lenght1, Length length2)
    {
      return lenght1._meters/length2._meters;
    }

    public static Length operator -(Length length)
    {
      return new Length(-length._meters);
    }

    public static Length operator +(Length length1, Length length2)
    {
      return new Length(length1._meters + length2._meters);
    }

    public static Length operator -(Length length1, Length length2)
    {
      return new Length(length1._meters - length2._meters);
    }

    public static bool operator ==(Length l1, Length l2)
    {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(l1, l2))
        return true;

      // If one is null, but not both, return false.
      if (((object) l1 == null) || ((object) l2 == null))
        return false;

      return l1._meters == l2._meters;
    }

    public static bool operator !=(Length l1, Length l2)
    {
      return !(l1 == l2);
    }

    public Length Abs()
    {
      return FromMeters(Math.Abs(_meters));
    }
  }
}