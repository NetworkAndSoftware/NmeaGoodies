using System;
using System.Net;
using System.Windows;
using Nmea0183;
using Nmea0183.Communications;
using Nmea0183.Messages;

namespace Experiment1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private readonly RepeatingSender _repeatingSender;

    public Apb Apb { get; set; }

    public MainWindow()
    {
      try
      {
        Apb = new Apb("SN")  // Electronic Positioning System, other/general
        {
          BOD = 160,
          BodMagneticOrTrue = Apb.MagneticOrTrue.Magnetic,
          Bearing = 160,
          BearingMagneticOrTrue = Apb.MagneticOrTrue.Magnetic,
          DestinationWayPointId = 1,
          Heading = 160,
          HeadingMagneticOrTrue = Apb.MagneticOrTrue.Magnetic,
          SteerTurn = Apb.Turn.Left,
          XTE = 0,
          XteUnits = Apb.Units.NauticalMiles,
        };
        InitializeComponent();

        _repeatingSender = new RepeatingSender() { Message = Apb };
        _repeatingSender.Start();
      }
      catch (Exception x)
      {
        MessageBox.Show(x.ToString());
        throw;
      }

    }

  }
}
