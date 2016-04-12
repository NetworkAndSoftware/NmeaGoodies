using System;

namespace Geometry
{
  public class Coordinate
  {
    public Latitude Latitude { get; set; }
    public Longitude Longitude { get; set; }

    public Coordinate(Latitude latitude, Longitude longitude)
    {
      Latitude = latitude;
      Longitude = longitude;
    }

    /// <summary>
    /// Great circle distance between two positions.
    /// 
    /// From http://williams.best.vwh.net/avform.htm:
    /// d=2*asin(sqrt((sin((lat1-lat2)/2))^2 + 
    ///              cos(lat1)*cos(lat2)*(sin((lon1-lon2)/2))^2))
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns>Angular distance along a great circle</returns>
    public Angle Distance(Coordinate coordinate)
    {
      return 2 * Angle.Asin(Math.Sqrt(Math.Pow(((Latitude - coordinate.Latitude) / 2).Sin(), 2)
        + Latitude.Cos()*coordinate.Latitude.Cos() * Math.Pow(((Longitude - coordinate.Longitude) / 2).Sin(), 2)));
    }

    /// <summary>
    /// Initial course of great circle going to coordinate
    /// 
    /// From http://williams.best.vwh.net/avform.htm:
    /// 
    /// mod
    /// ( atan2
    ///   ( sin(lon1-lon2) * cos(lat2)
    ///   , cos(lat1)*sin(lat2) - sin(lat1)*cos(lat2)*cos(lon1-lon2)
    ///   )
    /// , 2*pi
    /// )
    /// </summary>
    /// <param name="coordinate">Target coordinate that we want to travel to</param>
    /// <returns>initial bearing</returns>
    public Bearing InitialCourse(Coordinate coordinate)
    {
      // TODO: resolve edge case: close to the poles
      // if (Latitude.Cos() < .0001)
      //  return Latitude.Hemisphere == Latitude.HemisphereType.North ? Bearing.South : Bearing.North;

      return new Bearing(Angle.Atan2((coordinate.Longitude - Longitude).Sin() * coordinate.Latitude.Cos(), Latitude.Cos()*coordinate.Latitude.Sin() -
                                                                                                           Latitude.Sin()*coordinate.Latitude.Cos()*(coordinate.Longitude - Longitude).Cos()));
    }

    public Coordinate GreatCircle(AngularVector angularVector)
    {
      return angularVector.GreatCircle(this);
    }

    public Coordinate Rhumb(AngularVector angularVector)
    {
      return angularVector.Rhumb(this);
    }
  }
}
