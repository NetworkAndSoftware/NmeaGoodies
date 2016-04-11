namespace Geometry
{
  public class Latitude : Angle
  {
    public Latitude(Angle angle) : base(angle)
    {}

    public new static Latitude FromDegrees(double degrees)
    {
      return new Latitude(Angle.FromDegrees(degrees));
    }

    public new static Latitude FromDegrees(double degrees, double minutes, double seconds)
    {
      return new Latitude(Angle.FromDegrees(degrees, minutes, seconds));
    }

    public new static Latitude FromRadians(double radians)
    {
      return new Latitude(Angle.FromRadians(radians));
    }

    public enum HemisphereType
    {
      North,
      South
    };

    public HemisphereType Hemisphere
    {
      get { return _radians > 0 ? HemisphereType.North : HemisphereType.South; }
    }
  }
}
