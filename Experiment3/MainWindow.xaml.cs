﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Experiment3.Helpers;
using Experiment3.ViewModels;
using Nmea0183.Communications;
using Nmea0183.Constants;
using Nmea0183.Messages;

namespace Experiment3
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      ViewModel.MessageReader.Message += (message, datetime) =>
      { if (MessageName.Unknown != message.Name)
          Console.WriteLine($"{datetime.ToLocalTime():G} {message}");
      };

      if (!Debugger.IsAttached) // because it makes the debugger really slow if hook code is being executed after the program is paused
        SystemIdleHook.Enabled = true;
    }



    private MainViewModel ViewModel => (MainViewModel)this.DataContext;

    private void Main_Deactivated(object sender, EventArgs e)
    {
      if (Debugger.IsAttached)
        return;

      Topmost = false;

      var t = new DispatcherTimer() {Interval = TimeSpan.FromSeconds(1)};
      t.Tick += (o, args) =>
      {
        if (SystemIdleHook.IdleTime <= TimeSpan.FromSeconds(2))
          return;

        Topmost = false;
        Activate();
        t.Stop();
      };
      t.Start();
    }

    private void Main_Loaded(object sender, RoutedEventArgs e)
    {
      Activate();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
        DragMove();
    }
  }
}
