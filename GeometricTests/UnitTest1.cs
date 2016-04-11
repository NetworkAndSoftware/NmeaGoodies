using System;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeometricTests
{
   [TestClass]
   public class UnitTest1
   {
      private static readonly Coordinate Ourhouse = new Coordinate(Latitude.FromDegrees(49, 0, 51.55),
         Longitude.FromDegrees(123, 4, 39.87));

      private static readonly Coordinate PointRobertsMarina = new Coordinate(Latitude.FromDegrees(48, 58, 44.02),
         Longitude.FromDegrees(123, 4, 4.09));

      private static readonly Coordinate PottsburgCrossing = new Coordinate(Latitude.FromDegrees(30, 15, 45.57),
         Longitude.FromDegrees(81, 35, 30.09));

      private static readonly Coordinate MammasHuis = new Coordinate(Latitude.FromDegrees(51, 57, 49.22),
         Longitude.FromDegrees(-4, 30, 2.66));

      private static readonly Coordinate TsatsuCondos = new Coordinate(Latitude.FromDegrees(49, 1, 24.30), Longitude.FromDegrees(123, 6, 6.47));

      [TestMethod]
      public void ShouldCalculateDistance()
      {
         Length distance = Ball.EarthSurfaceApproximation.Distance(Ourhouse, PointRobertsMarina);
         Assert.IsTrue(distance < Length.FromMiles(2.6) && distance > Length.FromMiles(2.4));

         distance = Ball.EarthSurfaceApproximation.Distance(Ourhouse, PottsburgCrossing);

         Assert.IsTrue(distance > Length.FromMiles(2500) && distance < Length.FromMiles(2600));

         distance = Ball.EarthSurfaceApproximation.Distance(Ourhouse, MammasHuis);
         Assert.IsTrue(distance > Length.FromMiles(4800) && distance < Length.FromMiles(4850));
      }

      [TestMethod]
      public void ShouldCalculateInitialCourse()
      {
         Bearing course = Ourhouse.InitialCourse(PointRobertsMarina);

         Assert.IsTrue(course.Degrees > 169 && course.Degrees < 170);

         course = Ourhouse.InitialCourse(MammasHuis);

         Assert.IsTrue(course.Degrees > 28 && course.Degrees < 32);

        course = Ourhouse.InitialCourse(TsatsuCondos);

        Assert.IsTrue(course.Degrees > 290 && course.Degrees < 310);
    }

      [TestMethod]
      public void ShouldVectorRhumbWell()
      {
         var vector = new AngularVector(Ourhouse, PointRobertsMarina);

         Coordinate newlocation = Ourhouse.Rhumb(vector);

         Length distance = Ball.EarthSurfaceApproximation.Distance(newlocation, PointRobertsMarina);

         Assert.IsTrue(distance < Length.FromMeters(.5));

         vector = new AngularVector(Ourhouse, PottsburgCrossing);

         newlocation = Ourhouse.Rhumb(vector);

         distance = Ball.EarthSurfaceApproximation.Distance(newlocation, PottsburgCrossing);

         Assert.IsFalse(distance < Length.FromMeters(.5));
      }

      [TestMethod]
      public void ShouldVectorGreatCirclesWell()
      {
         var vector = new AngularVector(Ourhouse, PointRobertsMarina);

         Coordinate newlocation = Ourhouse.GreatCircle(vector);

         Length distance = Ball.EarthSurfaceApproximation.Distance(newlocation, PointRobertsMarina);

         Assert.IsTrue(distance < Length.FromMeters(.5));

         vector = new AngularVector(Ourhouse, MammasHuis);

         newlocation = Ourhouse.GreatCircle(vector);

         distance = Ball.EarthSurfaceApproximation.Distance(newlocation, MammasHuis);

         Assert.IsTrue(distance < Length.FromMeters(.5));
      }

      [TestMethod]
      public void ShouldCalculateDeviations()
      {
         var bearings = new BearingsForComparison(10, 20);
         Assert.IsTrue(bearings.DeviationCloseTo(10));
         bearings = new BearingsForComparison(20, 10);
         Assert.IsTrue(bearings.DeviationCloseTo(-10));
         bearings = new BearingsForComparison(350, 10);
         Assert.IsTrue(bearings.DeviationCloseTo(20));
         bearings = new BearingsForComparison(10, 350);
         Assert.IsTrue(bearings.DeviationCloseTo(-20));
         bearings = new BearingsForComparison(170, 190);
         Assert.IsTrue(bearings.DeviationCloseTo(20));
         bearings = new BearingsForComparison(190, 170);
         Assert.IsTrue(bearings.DeviationCloseTo(-20));
      }

      [TestMethod]
      public void EarthShouldCreateCartesianVectorFromAngularVector()
      {
         var angularVector = new AngularVector(Bearing.North, Angle.FromDegrees(1));
         CartesianVector cartesianVector = Ball.EarthSurfaceApproximation.Cartesian(angularVector);

         Assert.IsTrue(cartesianVector.X == Length.Zero);
         Assert.IsTrue(cartesianVector.Y > Length.Zero);

         angularVector = new AngularVector(Bearing.East, Angle.FromDegrees(1));
         cartesianVector = Ball.EarthSurfaceApproximation.Cartesian(angularVector);

         Assert.IsTrue(cartesianVector.X > Length.Zero);
         Assert.IsTrue(cartesianVector.Y < Length.FromMeters(1E-10));
      }

      [TestMethod]
      public void EarthShouldCreateAngularVectorFromCartesianVector()
      {
         var cartesianvector = new CartesianVector(Length.FromMeters(0), Length.FromMeters(1));
         AngularVector angularvector = Ball.EarthSurfaceApproximation.AngularVector(cartesianvector);

         Assert.IsTrue((angularvector.Bearing.Abs() - Bearing.North.Abs()).Abs() < Angle.FromDegrees(1e-6));

         cartesianvector = new CartesianVector(Length.FromMeters(1), Length.FromMeters(0));
         angularvector = Ball.EarthSurfaceApproximation.AngularVector(cartesianvector);

         Assert.IsTrue((angularvector.Bearing.Abs() - Bearing.East.Abs()).Abs() < Angle.FromDegrees(1e-6));
      }

      private struct BearingsForComparison
      {
         private readonly Bearing _bearing1;
         private readonly Bearing _bearing2;

         public BearingsForComparison(double degrees1, double degrees2)
         {
            _bearing1 = new Bearing(Angle.FromDegrees(degrees1));
            _bearing2 = new Bearing(Angle.FromDegrees(degrees2));
         }

         private Angle Deviation()
         {
            return _bearing2 - _bearing1;
         }

         public bool DeviationCloseTo(double degrees)
         {
            return Math.Abs(Deviation().Degrees - degrees) < 1;
         }
      }
   }
}