using System;
using System.Globalization;
using System.Net;
using System.Windows;
using Nmea0183;

namespace Experiment2
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
        Apb = new Apb("SN") // Electronic Positioning System, other/general
        {
          BOD = 160,
          BodMagneticOrTrue = Apb.MagneticOrTrue.Magnetic,
          DestinationWayPointId = 1,
          XTE = 0,
          XteUnits = Apb.Units.NauticalMiles,
        };
        Adjust();

        InitializeComponent();

        _messageSender = new MessageSender(IPAddress.Loopback, onBeforeSend: ModifyToKeepAutopilotOnTrack) { Message = Apb };
        _messageSender.Start();

      }
      catch (Exception x)
      {
        MessageBox.Show(x.ToString());
        throw;
      }

    }
    
    private static void ModifyToKeepAutopilotOnTrack(MessageBase messageBase)
    {
      var apb = (Apb) messageBase;

      apb.SteerDirection = apb.SteerDirection == Apb.Direction.Left ? Apb.Direction.Right : Apb.Direction.Left;
    }


    private void OnLeft(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
      Apb.BOD--;

      while (Apb.BOD < 0)
        Apb.BOD += 360;

      TextBox.Text = Apb.BOD.ToString(CultureInfo.InvariantCulture);
    }

    private void OnRight(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
      Apb.BOD++;

      while (Apb.BOD >= 360)
        Apb.BOD -= 360;

      TextBox.Text = Apb.BOD.ToString(CultureInfo.InvariantCulture);
    }

    private void Adjust()
    {
      Apb.Bearing = Apb.BOD;
      Apb.Heading = Apb.BOD;
      Apb.BearingMagneticOrTrue = Apb.BodMagneticOrTrue;
      Apb.HeadingMagneticOrTrue = Apb.BodMagneticOrTrue;
    }

    private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      Adjust();
    }

  }
}
