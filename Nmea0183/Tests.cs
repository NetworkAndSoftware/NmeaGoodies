using System;
using Geometry;
using Nmea0183.Messages;
using Nmea0183.Messages.Enum;
using Nmea0183.Messages.Interfaces;
using Xunit;

namespace Nmea0183
{
  public class Tests
  {
    [Fact]
    public void Apb()
    {
      var expected = new APB("SN") // Electronic Positioning System, other/general
      {
        SteerTurn = Turn.Left,
        BOD = 131,
        BodMagneticOrTrue = MagneticOrTrue.Magnetic,
        DestinationWayPointId = 1,
        XTE = 68,
        XteUnits = Units.NauticalMiles,
        Bearing = 90,
        BearingMagneticOrTrue = MagneticOrTrue.True,
        Heading = 123,
        HeadingMagneticOrTrue = MagneticOrTrue.Magnetic,
        ArrivalCircular = Flag.Void,
        ArrivalPerpendicular = Flag.Active
      };

      var actual = new APB("SN", "A,A,68.0000,L,N,V,A,131.0000,M,001,90.0000,T,123.0000,M".Split(','));
      Assert.Equal(expected.BOD, actual.BOD);
      Assert.Equal(expected.ToString(), actual.ToString());
    }


    [Fact]
    public void Rmc()
    {
      var expected = new RMC("SN")
      {
        Status = Flag.Active,
        Position = new Position()
        {
          Latitude = 49.00859,
          LatitudeHemisphere = NorthSouth.North,
          Longitude = 123.04663,
          LongitudeHemisphere = EastWest.West,
        },
        SOG = 0.111F,
        TMG = 200.928F,
        // missing Magnetic Variation
        // missing Magnetic Variation sign
        // missing date and time
      };
      var actual = new RMC("SN", ",A,4900.859,N,12304.663,W,0.111,200.928,,0.000,".Split(','));
      Assert.InRange(actual.Position.Latitude, expected.Position.Latitude - .00001, expected.Position.Latitude + .00001);
      Assert.Equal(expected.ToString(), actual.ToString());

      actual = (RMC) MessageBase.Parse("$ECRMC,205351,A,4857.077,N,12303.894,W,4.389,182.020,030416,16.445,E*40");

      Assert.InRange(actual.MagneticVariation, 16.444999, 16.445001);

      // A different real life example. Time has two or three decimals, magnetic variation fields are missing and this is NMEA 2.3, with an extra field D or A.
      actual = (RMC) MessageBase.Parse("$GPRMC,222248.00,A,4857.49084,N,12302.35656,W,3.018,114.14,090416,,,D*7A");
      Assert.NotNull(actual);
    }

    [Fact]
    public void Hdm()
    {
      var expected = new HDM("AP")
      {
        Heading = 153.4,
        Type = MagneticOrTrue.Magnetic
      };
      var actual = (HDM) MessageBase.Parse("$APHDM,153.40,M*00");

      Assert.Equal(expected.Heading, actual.Heading);
      Assert.Equal(expected.Type, actual.Type);

      Assert.Equal(expected.ToString(), actual.ToString());

      Assert.InRange(actual.Heading, 153.399999, 153.400001);

    }



    [Fact]
    public void Gbs()
    {
      var actual = MessageBase.Parse("$GPGBS,222247.00,2.0,1.5,2.7,,,,*41");

    }

    [Fact]
    public void Gll()
    {
      var actual = MessageBase.Parse("$GPGLL,4857.49115,N,12302.35773,W,222247.00,A,D*7C");
    }

    [Fact]
    public void Gga()
    {
      var actual = MessageBase.Parse("$GPGGA,222247.00,4857.49115,N,12302.35773,W,2,12,0.67,-3.8,M,-17.7,M,,0000*47");
    }

    [Fact]
    public void ShouldBeAbleToReadAllCommandsInFileWithoutException()
    {
      string line;

      // Read the file and display it line by line.
      System.IO.StreamReader file = new System.IO.StreamReader(@"..\..\..\doc\autopilot-sample.txt");

      int linenumber = 0;

      while ((line = file.ReadLine()) != null)
      {
        linenumber++;

        MessageBase message;

        if (line != @"$PSMDOV,1" // Shipmodul overflow
            && !line.StartsWith("$,") // nonsense I haven't figured out yet
            && !line.StartsWith("!AI") // AIS stuff
            && !line.StartsWith("!") // weed out other garbage
          )
          message = MessageBase.Parse(line);
      }

      file.Close();
    }

    double DistanceInMeters(Coordinate coordinate1, Coordinate coordinate2)
    {
      return Ball.EarthSurfaceApproximation.Distance(coordinate1.Distance(coordinate2)).Meters();
    }

    [Fact]
    public void PositionShouldCast()
    {
      var onemilimeter = .001;

      Coordinate actual = new Position("4900", "N", "12300", "W");
      var expected = new Coordinate(Latitude.FromDegrees(49), Longitude.FromDegrees(-123));
      Assert.InRange(DistanceInMeters(expected, actual), 0, onemilimeter); 

      actual = new Position()
      {
        Latitude = 49,
        LatitudeHemisphere = NorthSouth.South,
        Longitude = 123,
        LongitudeHemisphere = EastWest.West
      };
      expected = new Coordinate(Latitude.FromDegrees(-49), Longitude.FromDegrees(-123));
      Assert.InRange(DistanceInMeters(expected, actual), 0, onemilimeter);

      actual = new Position()
      {
        Latitude = 49,
        LatitudeHemisphere = NorthSouth.North,
        Longitude = 123,
        LongitudeHemisphere = EastWest.East
      };
      expected = new Coordinate(Latitude.FromDegrees(49), Longitude.FromDegrees(123));
      Assert.InRange(DistanceInMeters(expected, actual), 0, 1);

      Position pexpected = new Position("4900", "N", "12300", "W");
      Position pactual = (Coordinate) pexpected; // explicit cast to Coordinate, then implicit back to Position
      Assert.InRange(pactual.Latitude, 48.9, 49.1);
      Assert.Equal(pactual.LatitudeHemisphere, NorthSouth.North);
      Assert.InRange(pactual.Longitude, 122.9, 123.1);
      Assert.Equal(pactual.LongitudeHemisphere, EastWest.West);

    }

  }
}
 