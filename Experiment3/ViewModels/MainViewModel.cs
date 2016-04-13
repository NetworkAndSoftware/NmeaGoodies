using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using Experiment3.Helpers;
using Nmea0183;

namespace Experiment3.ViewModels
{
  class MainViewModel
  {
    private readonly MagneticContext _magneticContext = new MagneticContext(16.45);

    public MainViewModel()
    {
      AutopilotControl = new AutopilotControl(_magneticContext);
      Stats = new Stats(_magneticContext);
      Stats.PropertyChanged += CopyHeadingToAutopilot;

      // TODO: Ugh.
      BaseWithMagneticContext.MagneticContext = _magneticContext;
    }

    public Stats Stats { get; }
    public AutopilotControl AutopilotControl { get; }

    private void CopyHeadingToAutopilot(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      if (AutopilotControl.Enabled && !AutopilotControl.CopyCurrentHeading)
        return;

      if ("Heading" != propertyChangedEventArgs.PropertyName)
        return;

      AutopilotControl.Heading = Stats.Heading.Value;
    }

    public DelegateCommand WindowCloseCommand { get; } = new DelegateCommand(() => Application.Current.Shutdown());
    public DelegateCommand HelpCommand { get; } = new DelegateCommand(() => new HelpWindow().ShowDialog());


  }
}