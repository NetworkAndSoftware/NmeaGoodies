using System;
using System.Net;
using System.Windows;
using Nmea0183;

namespace Experiment1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private readonly MessageSender _messageSender;

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
          SteerDirection = Apb.Direction.Left,
          XTE = 0,
          XteUnits = Apb.Units.NauticalMiles,
        };
        InitializeComponent();

        _messageSender = new MessageSender(IPAddress.Loopback) { Message = Apb };
        _messageSender.Start();
      }
      catch (Exception x)
      {
        MessageBox.Show(x.ToString());
        throw;
      }

    }

  }
}
