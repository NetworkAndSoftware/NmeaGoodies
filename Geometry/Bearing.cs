namespace Geometry
{
  public class Bearing : Angle
  {
    public static readonly Bearing South = new Bearing(Angle.Pi);
    public static readonly Bearing North = new Bearing(Angle.FromRadians(0));
    public static readonly Bearing East = new Bearing(Angle.Pi / 2);
    public static readonly Bearing West = new Bearing(Angle.Pi * 1.5);

    public Bearing(Angle value) : base(Normalize(value))
    {
    }

    static private Angle Normalize(Angle angle)
    {
      var a = angle%(2*Angle.Pi);
      if (a < Angle.Zero)
        a += 2*Angle.Pi;
      return a;
    }

    public static Bearing operator -(Bearing bearing, Angle angle)
    {
      return new Bearing((Angle)bearing - angle);
    }

    /// <summary>
    /// Calculates the deviation between two bearings
    /// </summary>
    /// <returns>Either a positive or a negative angle</returns>
    public static Angle operator -(Bearing bearing1, Bearing bearing2)
    {
      var angle = FromRadians(bearing1.Radians - bearing2.Radians);
      if (angle.Abs() > Pi)
        angle = -(angle.Sign() * 2 * Pi - angle);
      return angle;
    }
  }
}
