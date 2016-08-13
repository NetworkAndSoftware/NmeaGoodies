using System;
using System.Collections.Generic;
using System.Linq;

namespace DepthGauge
{
  public class DepthConversions
  {
    private Dictionary<Unit, Tuple<double, string>> _unitdetails = new Dictionary<Unit, Tuple<double, string>>()
    {
      {Unit.Meters, new Tuple<double, string>(1, "m") },
      {Unit.Feet, new Tuple<double, string>(3.28084, "ft")},
      {Unit.Fathoms, new Tuple<double, string>(0.546807, "ftm.")}
    };

    private Unit _currentunit;
    private static readonly IEnumerable<Unit> UnitValues = Enum.GetValues(typeof(Unit)).Cast<Unit>();

    public DepthConversions(Unit initialunit = Unit.Feet)
    {
      _currentunit = initialunit;
    }

    public double ConvertFromMeters(double depth)
    {
      return depth * _unitdetails[_currentunit].Item1;
    }

    public string GetUnitLabel()
    {
      return _unitdetails[_currentunit].Item2;
    }

    public void NextUnit()
    {
      _currentunit++;
      if (_currentunit > UnitValues.Max())
        _currentunit = UnitValues.Min();
    }
    
    public enum Unit
    {
      Meters,
      Feet,
      Fathoms
    };
  }
}