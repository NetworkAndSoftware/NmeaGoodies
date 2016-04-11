using System;

namespace Geometry
{
  public class AngularVector
  {
    static readonly public AngularVector Zero = new AngularVector(Bearing.North, Angle.Zero);
    public Bearing Bearing;
    public Angle Distance;

    public AngularVector(Bearing bearing, Angle distance)
    {
      Bearing = bearing;
      Distance = distance;
    }

    public AngularVector(Coordinate coordinate1, Coordinate coordinate2)
    {
      Bearing = coordinate1.InitialCourse(coordinate2);
      Distance = coordinate1.Distance(coordinate2);
    }

    /// <summary>
    /// lat =asin(sin(lat1)*cos(d)+cos(lat1)*sin(d)*cos(tc))
    /// dlon=atan2(sin(tc)*sin(d)*cos(lat1),cos(d)-sin(lat1)*sin(lat))
    /// lon=mod( lon1-dlon +pi,2*pi )-pi
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public Coordinate GreatCircle(Coordinate coordinate)
    {
      var latitude = new Latitude(
        Angle.Asin(coordinate.Latitude.Sin()*Distance.Cos() + coordinate.Latitude.Cos()*Distance.Sin()*Bearing.Cos()));

      var dlon = Angle.Atan2(Bearing.Sin()*Distance.Sin()*coordinate.Latitude.Cos(),
                             Distance.Cos() - coordinate.Latitude.Sin()*latitude.Sin());

      var longitude = new Longitude((coordinate.Longitude - dlon + Angle.Pi)%(2*Angle.Pi) - Angle.Pi);

      return new Coordinate(latitude, longitude);
    }

    /// <summary>
    ///   lat= lat1+d*cos(tc)
    ///   IF (abs(lat) > pi/2) "d too large. You can't go this far along this rhumb line!"
    ///   IF (abs(lat-lat1) <![CDATA[<]]> sqrt(TOL)){
    ///    q=cos(lat1)
    ///   } ELSE {
    ///     dphi=log(tan(lat/2+pi/4)/tan(lat1/2+pi/4))
    ///     q= (lat-lat1)/dphi
    ///   }
    ///   dlon=-d*sin(tc)/q
   ///    lon=mod(lon1+dlon+pi,2*pi)-pi
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public Coordinate Rhumb(Coordinate coordinate)
    {
      var latitude = new Latitude(coordinate.Latitude + Bearing.Cos()*Distance);
      
      if (latitude.Abs() > Angle.Pi / 2)
        throw new ApplicationException("Distance too large. You can't go this far along this rhumb line!");

      double q;
      if ((latitude - coordinate.Latitude).Abs().Radians < 1E-15)
        q = coordinate.Latitude.Cos();
      else
      {
        var dphi = Angle.FromRadians(Math.Log((latitude/2 + Angle.Pi/4).Tan() / (coordinate.Latitude/2 + Angle.Pi/4).Tan()));
        q = (latitude - coordinate.Latitude) / dphi;
      }

      var dlon = -Distance *Bearing.Sin()/q;

      var longitude = new Longitude((coordinate.Longitude + dlon + Angle.Pi) % (2 * Angle.Pi) - Angle.Pi);
      return new Coordinate(latitude, longitude);
    }
  }
}
