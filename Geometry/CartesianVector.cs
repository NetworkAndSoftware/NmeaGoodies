using System;

namespace Geometry
{
  public class CartesianVector
  {
    public Length X;
    public Length Y;

    public CartesianVector(Length x, Length y)
    {
      X = x;
      Y = y;
    }
    
    public CartesianVector()
    {
    }

    public Length Hypotenuse()
    {
      return Sqrt(Square(X) + Square(Y));
    }

    private Length Square(Length l)
    {
      var m = l.Meters();
      return Length.FromMeters(m*m);
    }

    private Length Sqrt(Length l)
    {
      return Length.FromMeters(Math.Sqrt(l.Meters()));
    }

    public Angle Atan2()
    {
      return Angle.Atan2(X, Y);
    }
  }
}