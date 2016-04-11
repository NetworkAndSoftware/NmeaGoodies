namespace Geometry
{
   public class Time
   {
      public static readonly Time Second = new Time(1);
      public static readonly Time Hour = new Time(3600);
      private readonly double _seconds;

      private Time(double seconds)
      {
         _seconds = seconds;
      }

      public static Time FromSeconds(double seconds)
      {
         return new Time(seconds);
      }

      /// <summary>
      ///    Create a Time object from number of system ticks
      /// </summary>
      /// <param name="ticks">each tick is 100 ns</param>
      /// <returns></returns>
      public static Time FromTicks(long ticks)
      {
         return new Time(ticks*100e-9);
      }

      public static Time FromHours(double hours)
      {
         return new Time(hours*3600);
      }

      public static double operator /(Time time1, Time time2)
      {
         return time1._seconds/time2._seconds;
      }

      public static Time operator *(Time time, double d)
      {
         return new Time(time._seconds*d);
      }
   }
}