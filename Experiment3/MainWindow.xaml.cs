using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Experiment3.Helpers;
using Experiment3.ViewModels;
using Nmea0183.Communications;

namespace Experiment3
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    const int POLLINGINTERVAL = 250;

    public MainWindow()
    {
      InitializeComponent();
      MessageDispatcher.IncomingMessage += Console.WriteLine;
      WpfMessagePoller.SetInterval(POLLINGINTERVAL);
      SystemIdleHook.Enabled = true;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
        DragMove();
    }

    private MainViewModel ViewModel => (MainViewModel) this.DataContext;

    private void Main_Deactivated(object sender, EventArgs e)
    {
      var t = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1)};
      t.Tick += (o, args) =>
      {
        if (SystemIdleHook.IdleTime <= TimeSpan.FromSeconds(2))
          return;

        Activate();
        t.Stop();
      };
      t.Start();
    }

  }
}
