using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Experiment3.Annotations;
using Experiment3.Helpers;
using Nmea0183;
using Nmea0183.Communications;
using Nmea0183.Messages;
using Nmea0183.Messages.Enum;

namespace Experiment3.ViewModels
{
  internal class AutopilotControl : INotifyPropertyChanged
  {
    private readonly MagneticContext _magneticContext;


    public AutopilotControl(MagneticContext magneticContext)
    {
      _magneticContext = magneticContext;
      _apb = new APB("SN");
      _heading = new MagneticMessageCompassValue(0);
      EnabledCommand = new DelegateCommand(() => Enabled = !Enabled);
      RightCommand = new DelegateCommand(Right);
      LeftCommand = new DelegateCommand(Left);
      MagneticCommand = new DelegateCommand(() => HeadingMagnetic = true);
      TrueCommand = new DelegateCommand(() => HeadingMagnetic = false);
      SetToHeadingCommand = new DelegateCommand(() => CopyCurrentHeading = !CopyCurrentHeading);
      
      _periodicalMessageSender = new RepeatingSender(onBeforeSend: OnBeforeAPBSend) { Message = _apb };
    }

    private void OnBeforeAPBSend(MessageBase message)
    {
      _apb.SteerTurn = _apb.SteerTurn == Turn.Left ? Turn.Right : Turn.Left;
      UpdateApb();
      Console.WriteLine($"Sending {_apb}");
    }

    private void UpdateApb()
    {
      _apb.Heading = Heading;
      _apb.Bearing = Heading;
      _apb.BOD = Heading;
    }


    public DelegateCommand LeftCommand { get; }
    public DelegateCommand RightCommand { get; }
    public DelegateCommand EnabledCommand { get; }
    public DelegateCommand MagneticCommand { get; }
    public DelegateCommand TrueCommand { get; }
    public DelegateCommand SetToHeadingCommand { get; }


    private readonly APB _apb;
    private IMessageCompassValue _heading;
    private bool _enabled;
    private readonly RepeatingSender _periodicalMessageSender;
    private bool _copyCurrentHeading;

    public IMessageCompassValue Heading
    {
      get { return _heading; }
      set
      {
        _heading = value;
        InvokePropertyChanged();
      }
    }

    private void InvokePropertyChanged()
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HeadingValue"));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HeadingMagnetic"));
    }


    public double? HeadingValue
    {
      get { return _heading?.Value; }
      set
      {
        if (null == value)
          _heading = null;
        else
          _heading.Value = value.Value;
      }
    }

    public bool HeadingMagnetic
    {
      get { return _heading.IsMagnetic; }
      set
      {
        if (value ==_heading.IsMagnetic)
          return;

        _heading = value ? (IMessageCompassValue) _magneticContext.Magnetic(_heading) : _magneticContext.True(_heading);

        InvokePropertyChanged();
      }
    }

    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        _enabled = value;
        if (_enabled)
          _periodicalMessageSender.Start();
        else
          _periodicalMessageSender.Stop();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Enabled"));
      }
    }

    public bool CopyCurrentHeading
    {
      get { return _copyCurrentHeading; }
      set
      {
        _copyCurrentHeading = value; 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CopyCurrentHeading"));
      }
    }

    private void Left()
    {
      if (!Enabled || CopyCurrentHeading)
        return;

      Heading.Value = Math.Round(Heading.Value) - 1;

      while (Heading.Value < 0)
        Heading.Value += 360;

      InvokePropertyChanged();
    }

    private void Right()
    {
      if (!Enabled || CopyCurrentHeading)
        return;

      Heading.Value = Math.Round(Heading.Value) + 1;

      while (Heading.Value >= 360)
        Heading.Value -= 360;

      InvokePropertyChanged();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }


}
