using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Nmea0183.Communications;
using Nmea0183.Constants;
using Nmea0183.Messages;

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
      
      if ("Heading" == propertyChangedEventArgs.PropertyName)
        AutopilotControl.Heading = Stats.Heading;
    }

    public DelegateCommand<Window> WindowCloseCommand { get; } = new DelegateCommand<Window>(o => { o.Close(); });
  }
}