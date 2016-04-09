using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nmea0183;
using Nmea0183.Communications;
using Nmea0183.Constants;
using Nmea0183.Messages;

namespace Experiment2
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private const int POLLINGINTERVAL = 250;
    private const int MINIMUM_TMG_SPEED = 1;
    private readonly TimeSpan _updateStaleTime = TimeSpan.FromSeconds(3);

    public MainWindow()
    {
      try
      {
        Apb = new APB("SN") // Electronic Positioning System, other/general
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
        MessageDispatcher.IncomingMessage += ShowMessage;
        WpfMessagePoller.SetInterval(POLLINGINTERVAL);
      }
      catch (Exception x)
      {
        MessageBox.Show(x.ToString());
        throw;
      }
    }

    public APB Apb { get; set; }

    private readonly Dictionary<MessageName, Tuple<MessageBase, DateTime>> lastUpdate = new Dictionary<MessageName, Tuple<MessageBase, DateTime>>(); 

    private void ShowMessage(MessageBase message)
    {
      
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (message.Name)
        {
          case MessageName.RMC:
            var rmc = (RMC)message;
            LabelTmg.Content = rmc.SOG > MINIMUM_TMG_SPEED ? $"{rmc.TMG:F1}" : $"Go faster than {MINIMUM_TMG_SPEED} knots."; // tmg not accurate when going slow
            UpdateDeviation();
            break;            
          case MessageName.HDM:
            var hdm = (HDM) message;
            LabelHeading.Content = $"{hdm.Heading} {hdm.Type}";
            break;
        }

        lastUpdate[message.Name] = new Tuple<MessageBase, DateTime>(message, DateTime.UtcNow);

      if (IsStale(MessageName.RMC))
        LabelTmg.Content = "Stale";

      if (IsStale(MessageName.HDM))
        LabelHeading.Content = "Stale";

    }

    private ulong _samples = 0;
    private double _meandeviation = 0;

    private bool IsStale(MessageName message) => !lastUpdate.ContainsKey(message) || DateTime.UtcNow - lastUpdate[message].Item2 > _updateStaleTime;

    void UpdateDeviation()
    {
      if (IsStale(MessageName.RMC) || IsStale(MessageName.HDM))
        return;

      var rmc = (RMC) lastUpdate[MessageName.RMC].Item1;
      
      if (!(rmc.SOG > MINIMUM_TMG_SPEED))
        return;

      var hdm = (HDM)lastUpdate[MessageName.HDM].Item1;

      var deviation = hdm.Heading - rmc.TMG;

      _meandeviation = (deviation + _samples * _meandeviation)/_samples + 1;

      _samples++;

      LabelDeviation.Content = deviation;
      LabelSamples.Content = _samples;
      LabelCorrectedHeading.Content = hdm.Heading - deviation;
    }


    private static void ModifyToKeepAutopilotOnTrack(MessageBase messageBase)
    {
      var apb = (APB) messageBase;

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