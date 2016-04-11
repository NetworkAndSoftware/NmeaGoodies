using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Experiment3.Helpers;
using Nmea0183.Communications;
using Nmea0183.Constants;
using Nmea0183.Messages;
using Nmea0183.Messages.Enum;

namespace Experiment3.ViewModels
{
  class MainViewModel
  {
    public MainViewModel()
    {
      Stats.PropertyChanged += SetAutopilotHeadingToIncomingHeading;
    }

    public Stats Stats { get; } = new Stats();
    public AutopilotControl AutopilotControl { get; } = new AutopilotControl();

    private void SetAutopilotHeadingToIncomingHeading(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      if (AutopilotControl.Enabled)
        return;

      if ("Heading" != propertyChangedEventArgs.PropertyName)
        return;

      AutopilotControl.Heading = Stats.Heading;
      AutopilotControl.MagneticOrTrue = MagneticOrTrue.Magnetic;
    }


    public DelegateCommand<Window> WindowCloseCommand { get; } = new DelegateCommand<Window>(o => { o.Close(); });
  }
}