using Nmea0183.Messages;
using Xunit;

namespace Nmea0183
{
  public class Tests
  {
    [Fact]
    public void Apb()
    {
      var expected = new Apb("SN") // Electronic Positioning System, other/general
      {
        SteerTurn = MessageBase.Turn.Left,
        BOD = 131,
        BodMagneticOrTrue = MessageBase.MagneticOrTrue.Magnetic,
        DestinationWayPointId = 1,
        XTE = 68,
        XteUnits = MessageBase.Units.NauticalMiles,
        Bearing = 90,
        BearingMagneticOrTrue = MessageBase.MagneticOrTrue.True,
        Heading = 123,
        HeadingMagneticOrTrue = MessageBase.MagneticOrTrue.Magnetic,
        ArrivalCircular = MessageBase.Flag.Void,
        ArrivalPerpendicular = MessageBase.Flag.Active
      };

      var actual = new Apb("SN", "A,A,68.0000,L,N,V,A,131.0000,M,001,90.0000,T,123.0000,M".Split(','));
      Assert.Equal(expected.BOD, actual.BOD);
      Assert.Equal(expected.ToString(), actual.ToString());
    }


    [Fact]
    public void Rmc()
    {
      var expected = new RMC("SN")
      {
        Status = MessageBase.Flag.Active,
        Latitude = 4900.859F,
        LatitudeHemisphere = MessageBase.NorthSouth.North,
        Longitude = 12304.663F,
        LongitudeHemisphere = MessageBase.EastWest.West,
        SOG = 0.111F,
        TMG = 200.928F,
        // missing Magnetic Variation
        // missing Magnetic Variation sign
        // missing date and time
      };
      var actual = new RMC("SN", ",A,4900.859,N,12304.663,W,0.111,200.928,,0.000,".Split(','));
      Assert.Equal(expected.Latitude, actual.Latitude);
      Assert.Equal(expected.ToString(), actual.ToString());

      actual = (RMC) MessageBase.Parse("$ECRMC,205351,A,4857.077,N,12303.894,W,4.389,182.020,030416,16.445,E*40");

      Assert.InRange(actual.MagneticVariation, 16.444999, 16.445001);

    }
  }
}