using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Experiment3.Annotations;
using Experiment3.Helpers;
using Nmea0183.Communications;
using Nmea0183.Messages;
using Nmea0183.Messages.Enum;

namespace Experiment3.ViewModels
{
  internal class AutopilotControl : INotifyPropertyChanged
  {
    public AutopilotControl()
    {
      _apb = new APB("SN");
      EnabledCommand = new DelegateCommand(() => Enabled = !Enabled);
      RightCommand = new DelegateCommand(Right);
      LeftCommand = new DelegateCommand(Left);

      _periodicalMessageSender = new RepeatingSender(onBeforeSend: (message) => _apb.SteerTurn = _apb.SteerTurn == Turn.Left ? Turn.Right : Turn.Left) { Message = _apb };
    }

    public DelegateCommand LeftCommand { get; }
    public DelegateCommand RightCommand { get; }
    public DelegateCommand EnabledCommand { get; }

    private readonly APB _apb;
    private double _heading;
    private MagneticOrTrue _magneticOrTrue;
    private bool _enabled;
    private readonly RepeatingSender _periodicalMessageSender;
    private string _talkedId;

    public double Heading
    {
      get { return _heading; }
      set
      {
        _heading = value;
        if (Enabled)
          UpdateApb();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Heading"));
      }
    }

    public MagneticOrTrue MagneticOrTrue
    {
      get { return _magneticOrTrue; }
      set
      {
        _magneticOrTrue = value;
        if (Enabled)
          UpdateApb();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagneticOrTrue"));
      }
    }
    
    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        _enabled = value; 
        UpdateApb();
        if (_enabled)
          _periodicalMessageSender.Start();
        else
          _periodicalMessageSender.Stop();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Enabled"));
      }
    }

    private void UpdateApb()
    {
      _apb.Heading = Heading;
      _apb.HeadingMagneticOrTrue = MagneticOrTrue;
      _apb.Bearing = Heading;
      _apb.BearingMagneticOrTrue = MagneticOrTrue;
      _apb.BOD = Heading;
      _apb.BodMagneticOrTrue = MagneticOrTrue;
    }

    private void Left()
    {
      Heading--;

      while (Heading < 0)
        Heading += 360;

      //TextBox.Text = Heading.ToString(CultureInfo.InvariantCulture);
    }

    private void Right()
    {
      Heading++;

      while (Heading >= 360)
        Heading -= 360;

      //TextBox.Text = Heading.ToString(CultureInfo.InvariantCulture);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
