using System;

namespace Geometry
{
   public class Angle
   {
      private const double RADIANSPERDEGREE = Math.PI/180;

      public static readonly Angle Pi = FromRadians(Math.PI);
      public static readonly Angle FullCircle = 2*Pi;
      public static Angle Zero = FromRadians(0);

      protected readonly double _radians;

      protected Angle(double radians)
      {
         _radians = radians;
      }

      protected Angle(Angle angle)
      {
         _radians = angle._radians;
      }

      public double Degrees
      {
         get { return _radians/RADIANSPERDEGREE; }
      }

      public double Radians
      {
         get { return _radians; }
      }

      public static Angle FromDegrees(double degrees, double minutes, double seconds)
      {
         return FromDegrees(Math.Sign(degrees)*(Math.Abs(degrees) + (minutes + seconds/60)/60));
      }

      public static Angle FromDegrees(double degrees)
      {
         return new Angle(degrees*RADIANSPERDEGREE);
      }

      public static Angle FromRadians(double radians)
      {
         return new Angle(radians);
      }

      public double Cos()
      {
         return Math.Cos(_radians);
      }

      public double Sin()
      {
         return Math.Sin(_radians);
      }

      public static Angle Asin(double d)
      {
         return new Angle(Math.Asin(d));
      }

      public static Angle Atan2(Length y, Length x)
      {
         return Atan2(y.Meters(), x.Meters());
      }

      public static Angle Atan2(double y, double x)
      {
         return new Angle(Math.Atan2(y, x));
      }


      public static Angle operator %(Angle angle1, Angle angle2)
      {
         return new Angle(angle1._radians%angle2._radians);
      }

      public static Angle operator *(double d, Angle angle)
      {
         return new Angle(angle._radians*d);
      }

      public static Angle operator *(Angle angle, double d)
      {
         return d*angle;
      }

      public static Angle operator -(Angle angle1, Angle angle2)
      {
         return new Angle(angle1._radians - angle2._radians);
      }

      public static Angle operator +(Angle angle1, Angle angle2)
      {
         return new Angle(angle1._radians + angle2._radians);
      }

      public static Angle operator /(Angle angle, double d)
      {
         return new Angle(angle._radians/d);
      }

      public static double operator /(Angle angle1, Angle angle2)
      {
         return angle1._radians/angle2._radians;
      }

      public static bool operator >(Angle angle1, Angle angle2)
      {
         return angle1._radians > angle2._radians;
      }

      public static bool operator <(Angle angle1, Angle angle2)
      {
         return angle2 > angle1;
      }

      public static bool operator >=(Angle angle1, Angle angle2)
      {
         return angle1._radians >= angle2._radians;
      }

      public static bool operator <=(Angle angle1, Angle angle2)
      {
         return angle2 >= angle1;
      }

      public static Angle operator -(Angle angle)
      {
         return new Angle(-angle._radians);
      }

      public Angle Abs()
      {
         return FromRadians(Math.Abs(_radians));
      }

      public double Tan()
      {
         return Math.Tan(_radians);
      }

      public int Sign()
      {
         return Math.Sign(Radians);
      }
   }
}