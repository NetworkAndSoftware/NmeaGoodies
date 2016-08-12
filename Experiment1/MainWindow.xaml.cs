using System;
using System.Net;
using System.Windows;
using Nmea0183;
using Nmea0183.Communications;
using Nmea0183.Messages;
using Nmea0183.Messages.Enum;

namespace Experiment1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private readonly RepeatingSender _repeatingSender;

    public APB Apb { get; set; }

    public MainWindow()
    { var heading = new MagneticMessageCompassValue(160);
      try
      {
        Apb = new APB("SN")  // Electronic Positioning System, other/general
        {
          BOD = heading,
          Bearing = heading,
          DestinationWayPointId = 1,
          Heading = heading,
          SteerTurn = Turn.Left,
          XTE = 0,
          XteUnits = Units.NauticalMiles,
        };
        InitializeComponent();

        _repeatingSender = new RepeatingSender(new MessageSender("localhost", "serialout"), TimeSpan.FromSeconds(1)) { Message = Apb };
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
