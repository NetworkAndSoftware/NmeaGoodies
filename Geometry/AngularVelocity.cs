namespace Geometry
{
  public class AngularVelocity
  {
    private readonly Angle _angle;
    private readonly Time _time;

    private AngularVelocity(Angle angle, Time time)
    {
      _angle = angle;
      _time = time;
    }

    public static AngularVelocity FromRadiansPerSecond(double d)
    {
      return new AngularVelocity(Angle.FromRadians(d), Time.Second);
    }

    public static AngularVelocity FromDegreesPerSecond(double d)
    {
      return new AngularVelocity(Angle.FromDegrees(d), Time.Second);
    }

    public double RadiansPerSecond()
    {
      return _angle.Radians*(Time.Hour/_time);
    }

    public double DegreesPerSecond()
    {
      return _angle.Degrees*(Time.Second/_time);
    }

    public static Angle operator *(AngularVelocity velocity, Time time)
    {
      return velocity._angle * (time / velocity._time) ;
    }

    public static Time operator /(Angle Angle, AngularVelocity velocity)
    {
      return velocity._time*(Angle/velocity._angle);
    }
  }
}
