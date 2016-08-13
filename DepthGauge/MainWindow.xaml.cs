﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Experiment3.Helpers;
using Nmea0183.Communications;
using Nmea0183.Messages.Interfaces;
using WpfGoodies;

namespace DepthGauge
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly DepthConversions _depthConversions;
    private double _depth;
    private readonly MessageReader _reader;

    public MainWindow()
    {
      InitializeComponent();
      _reader = new MessageReader("localhost", "serialin");

      _depthConversions = new DepthConversions();
      ShowUnit();

      _reader.Message += (message, datetime) =>
      {
        var havedepth = message as IHaveDepth;

        if (havedepth != null)
        {
          _depth = havedepth.Depth + havedepth.Offset;
          Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(ShowDepth));
        }
      };
    }

    private void Run_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      _depthConversions.NextUnit();
      ShowUnit();
      ShowDepth();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
        DragMove();
    }

    private void Main_Deactivated(object sender, EventArgs e)
    {
      if (Debugger.IsAttached)
        return;

      Topmost = false;

      var t = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
      t.Tick += (o, args) =>
      {
        if (SystemIdleHook.IdleTime <= TimeSpan.FromSeconds(2))
          return;

        Topmost = true;
        Activate();
        t.Stop();
      };
      t.Start();
    }

    private void Main_Loaded(object sender, RoutedEventArgs e)
    {
      Activate();
    }

    private void ShowUnit()
    {
      Unit.Text = _depthConversions.GetUnitLabel();
    }

    private void ShowDepth()
    {
      Depth.Text = _depthConversions.ConvertFromMeters(_depth).ToString("F1");
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      _reader.Dispose();
    }

    public DelegateCommand WindowCloseCommand { get; } = new DelegateCommand(() => Application.Current.Shutdown());
    public DelegateCommand HelpCommand { get; } = new DelegateCommand(() => new HelpWindow().ShowDialog());

  }
}