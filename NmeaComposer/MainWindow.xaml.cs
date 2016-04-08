using System.Windows;
using System.Windows.Controls;
using Nmea0183;
using NmeaComposer.ViewModels;

namespace NmeaComposer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    
    
    public MainWindow()
    { InitializeComponent();
    }

    private MainViewModel ViewModel
    {
      get
      {
        return (MainViewModel) DataContext;
      }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
     //ViewModel.CreateMessageFromBasic();
    }

  }
}
