using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nmea0183;
using Nmea0183.Communications;
using Nmea0183.Messages;

namespace Experiment2
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private const int POLLINGINTERVAL = 250;

    public MainWindow()
    {
      try
      {
        Apb = new Apb("SN") // Electronic Positioning System, other/general
        {
          BOD = 160,
          BodMagneticOrTrue = MessageBase.MagneticOrTrue.Magnetic,
          DestinationWayPointId = 1,
          XTE = 0,
          XteUnits = MessageBase.Units.NauticalMiles
        };
        Adjust();

        InitializeComponent();

        var periodicalMessageSender = new RepeatingSender(onBeforeSend: ModifyToKeepAutopilotOnTrack) {Message = Apb};
        periodicalMessageSender.Start();

        MessageDispatcher.IncomingMessage += Console.WriteLine;
        MessageDispatcher.IncomingMessage += ShowTrackMadeGood;
        WpfMessagePoller.SetInterval(POLLINGINTERVAL);
      }
      catch (Exception x)
      {
        MessageBox.Show(x.ToString());
        throw;
      }
    }

    public Apb Apb { get; set; }

    private void ShowTrackMadeGood(MessageBase message)
    {
      if (typeof (RMC) == message.GetType())
      {
        var rmc = (RMC) message;

        LabelTMG.Content = rmc.SOG > 1 ? $"{rmc.TMG:F1}" : "Go faster."; // tmg not accurate when going slow
      }
    }

    private static void ModifyToKeepAutopilotOnTrack(MessageBase messageBase)
    {
      var apb = (Apb) messageBase;

      apb.SteerTurn = apb.SteerTurn == MessageBase.Turn.Left ? MessageBase.Turn.Right : MessageBase.Turn.Left;
    }


    private void OnLeft(object sender, ExecutedRoutedEventArgs e)
    {
      Apb.BOD--;

      while (Apb.BOD < 0)
        Apb.BOD += 360;

      TextBox.Text = Apb.BOD.ToString(CultureInfo.InvariantCulture);
    }

    private void OnRight(object sender, ExecutedRoutedEventArgs e)
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

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      Adjust();
    }
  }
}