using System.ComponentModel;
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
      Stats.PropertyChanged += SetAutopilotHeadingToIncomingHeading;

      // TODO: Ugh.
      WithMagneticContext.MagneticContext = _magneticContext;
    }

    public Stats Stats { get; }
    public AutopilotControl AutopilotControl { get; }

    private void SetAutopilotHeadingToIncomingHeading(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      if (AutopilotControl.Enabled)
        return;

      if ("Heading" != propertyChangedEventArgs.PropertyName)
        return;

      AutopilotControl.Heading = Stats.Heading.Value;
    }

    public DelegateCommand<Window> WindowCloseCommand { get; } = new DelegateCommand<Window>(o => { o.Close(); });

    public DelegateCommand<Window> HelpCommand { get; } = new DelegateCommand<Window>((w) =>
    {
      var h = new HelpWindow();
      h.Show();
    });

  }
}