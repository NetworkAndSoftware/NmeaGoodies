using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
        DragMove();
    }

  }
}
