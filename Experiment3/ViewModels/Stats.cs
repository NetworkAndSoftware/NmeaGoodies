using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Experiment3.Annotations;
using Experiment3.Models;
using Geometry;
using Nmea0183.Communications;
using Nmea0183.Constants;
using Nmea0183.Messages;
using Nmea0183.Messages.Interfaces;

namespace Experiment3.ViewModels
{
  internal class Stats : INotifyPropertyChanged
  {
    private const double MINIMUM_SPEED_FOR_VALID_CALCULATIONS = 1;
    private readonly TimeSpan _minimumElapsedtimeForCalculations = TimeSpan.FromMilliseconds(100);
    private readonly Length _minimumDistanceForCalculations = Length.FromMeters(.001); // one millimeter

    private const double MAGNETIC_VARIATION = 16.45; // + E, -W, like OpenCPN

    public Stats()
    {
      MessageDispatcher.IncomingMessage += OnIncomingMessage;
      var timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
      timer.Tick += TimerTick;
      timer.Start();
    }
    
    private void TimerTick(object sender, EventArgs e)
    {
      CallPropertyChangedForStaleProperties();
    }

    private QuantityWithMetadata<Coordinate> _lastposition;

    private void OnUpdatePosition(QuantityWithMetadata<Coordinate> coordinate)
    {

      if (null == Sog || Sog.Source != QuantityWithMetadata<double>.SourceType.External || Sog.IsStale)
        // Try to calculate Sog from position data
        if (null != _lastposition && !_lastposition.IsStale)
        {
          var angulardistance = ((Coordinate) coordinate).Distance(_lastposition);
          var distance = Ball.EarthSurfaceApproximation.Distance(angulardistance).NauticalMiles();

          if (distance >= _minimumDistanceForCalculations.NauticalMiles())
          {
            var elapsed = DateTime.UtcNow - _lastposition.Updated;
            if (elapsed.HasValue && elapsed.Value >= _minimumElapsedtimeForCalculations)
            {
              Sog = distance/elapsed.Value.TotalHours;
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sog"));
            }
          }
        }

      if (null == Cog || Cog.Source != QuantityWithMetadata<double>.SourceType.External || Cog.IsStale)
      {
        // Try to calculate Cog from position data
        if (null != _lastposition && !_lastposition.IsStale)
        {
          var angulardistance = ((Coordinate)coordinate).Distance(_lastposition);
          var distance = Ball.EarthSurfaceApproximation.Distance(angulardistance).NauticalMiles();

          if (distance >= _minimumDistanceForCalculations.NauticalMiles())
          {
            if (null != Sog && Sog > MINIMUM_SPEED_FOR_VALID_CALCULATIONS)
            {
              var truecog = _lastposition.Value.InitialCourse(coordinate);
              Cog = truecog.Degrees - MAGNETIC_VARIATION;
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cog"));
              OnUpdateCog();
            }
          }
        }
      }

      _lastposition = coordinate;
    }

    private void OnUpdateCog()
    {
      if (null != Heading && !Heading.IsStale)
      {
        UpdateDeviation();
      }
    }

    private bool AnyStale(IEnumerable<QuantityWithMetadata<double>> properties)
    {
      return properties.Any(property => (null == property) || property.IsStale);
    }

    private ulong _samples;
    private double _meandeviation;

    void UpdateDeviation()
    {
      if (AnyStale(new []{Sog, Cog, Heading}))
        return;

      if (Sog < MINIMUM_SPEED_FOR_VALID_CALCULATIONS)
        return;

      var deviation = Heading.Value - Cog.Value;

      _meandeviation = (deviation + _samples * _meandeviation) / (_samples + 1);

      _samples++;

      MeanDeviation = _meandeviation;
      SampleCount = _samples;
      CorrectedHeading = Heading - MeanDeviation;

      if (PropertyChanged == null)
        return;

      PropertyChanged(this, new PropertyChangedEventArgs("MeanDeviation"));
      PropertyChanged(this, new PropertyChangedEventArgs("SampleCount"));
      PropertyChanged(this, new PropertyChangedEventArgs("CorrectedHeading"));
    }


    private void CallPropertyChangedForStaleProperties()
    {
      var properties =
        GetType()
          .GetProperties(BindingFlags.Instance | BindingFlags.Public)
          .Where(info => info.PropertyType.GetInterfaces().Contains(typeof(IQuantityWithMetaData)));

      foreach (var info in properties)
      {
        var property = (IQuantityWithMetaData)info.GetValue(this);
        if (null == property)
          continue;

        if (property.IsStale != property.UIToldItsStale)
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info.Name));

        property.UIToldItsStale = property.IsStale;
      }
    }

    private readonly Dictionary<MessageName, Tuple<MessageBase, DateTime>> _lastMessageByType = new Dictionary<MessageName, Tuple<MessageBase, DateTime>>();
    

    private void OnIncomingMessage(MessageBase message)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (message.Name)
      {
        case MessageName.HDM:
        {
          var hdm = (HDM) message;
          Heading = hdm.Heading;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Heading"));
        }
          break;

        case MessageName.RMC:
        {
          var rmc = (RMC) message;
          if (rmc.SOG > MINIMUM_SPEED_FOR_VALID_CALCULATIONS)
          {
            Cog = rmc.TMG;
            Cog.Source = QuantityWithMetadata<double>.SourceType.External;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cog"));
            Sog = rmc.SOG;
            Sog.Source = QuantityWithMetadata<double>.SourceType.External;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sog"));
            OnUpdateCog();
          }
        }
          break;
      }
      if (message is IHasPosition)
      {
        QuantityWithMetadata<Coordinate> c = (Coordinate) (message as IHasPosition).Position;
        c.Source = QuantityWithMetadata<Coordinate>.SourceType.External;
        OnUpdatePosition(c);
      }

      UpdateLastMessageDictionary(message);
    }

    private void UpdateLastMessageDictionary(MessageBase message)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (message.Name)
      {
        case MessageName.RMC:
        case MessageName.HDM:
        case MessageName.GGA:
        case MessageName.GLL:
          _lastMessageByType[message.Name] = new Tuple<MessageBase, DateTime>(message, DateTime.UtcNow);
          break;
      }
    }

    public QuantityWithMetadata<double> Heading { get; private set; }

    public QuantityWithMetadata<double> Cog { get; private set; }

    public QuantityWithMetadata<double> Sog { get; private set; }

    public double MeanDeviation { get; private set; }

    public ulong SampleCount { get; private set; }

    public QuantityWithMetadata<double> CorrectedHeading { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }




}
