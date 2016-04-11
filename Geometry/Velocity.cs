namespace Geometry
{
   public class Velocity
   {
      private readonly Length _length;
      private readonly Time _time;

      private Velocity(Length length, Time time)
      {
         _length = length;
         _time = time;
      }

      public static Velocity FromMeterPerSecond(double d)
      {
         return new Velocity(Length.FromMeters(d), Time.Second);
      }

      public static Velocity FromMPH(double d)
      {
         return new Velocity(Length.FromMiles(d), Time.Hour);
      }

      public static Velocity FromKnots(double d)
      {
         return new Velocity(Length.FromNauticalMiles(d), Time.Hour);
      }

      public double MPH()
      {
         return _length.Miles()*(Time.Hour/_time);
      }

      public double Knots()
      {
         return _length.NauticalMiles()*(Time.Hour/_time);
      }

      public double MetersPerSecond()
      {
         return _length.Meters()*(Time.Second/_time);
      }

      public static Length operator *(Velocity velocity, Time time)
      {
         return velocity._length*(time/velocity._time);
      }

      public static Time operator /(Length length, Velocity velocity)
      {
         return velocity._time*(length/velocity._length);
      }
   }
}