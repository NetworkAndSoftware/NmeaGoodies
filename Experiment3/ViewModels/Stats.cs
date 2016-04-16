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
using Newtonsoft.Json;
using Nmea0183;
using Nmea0183.Communications;
using Nmea0183.Constants;
using Nmea0183.Messages;
using Nmea0183.Messages.Interfaces;

namespace Experiment3.ViewModels
{
  internal class Stats : INotifyPropertyChanged
  {
    private const double MINIMUM_SPEED_FOR_VALID_CALCULATIONS = 1;
    private const string FILENAME_COMPASS_CORRECTION = @"compasscorrection.json";

    private readonly CompassCorrectionPersister _compassCorrectionPersister;

    private readonly Dictionary<MessageName, Tuple<MessageBase, DateTime>> _lastMessageByType =
      new Dictionary<MessageName, Tuple<MessageBase, DateTime>>();

    private MagneticContext _magneticContext;

    private readonly Length _minimumDistanceForCalculations = Length.FromMeters(.001); // one millimeter
    private readonly TimeSpan _minimumElapsedtimeForCalculations = TimeSpan.FromMilliseconds(100);

    private readonly CompassCorrection _compassCorrection;

    private QuantityWithMetadata<Coordinate> _lastposition;

    public Stats(MagneticContext magneticContext)
    {
      _magneticContext = magneticContext;
      _compassCorrection = new CompassCorrection(_magneticContext);
      _compassCorrectionPersister = new CompassCorrectionPersister(FILENAME_COMPASS_CORRECTION);
      try
      {
        _compassCorrectionPersister.Read(_compassCorrection);
      }
      catch (JsonSerializationException)
      {
        // eat it
      }


      MessageDispatcher.IncomingMessage += OnIncomingMessage;
      var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
      timer.Tick += TimerTick;
      timer.Start();
    }

    public QuantityWithMetadata<IMessageCompassValue> Heading { get; private set; }
    public QuantityWithMetadata<IMessageCompassValue> Cog { get; private set; }
    public QuantityWithMetadata<double> Sog { get; private set; }
    public QuantityWithMetadata<IMessageCompassValue> CorrectedHeading { get; private set; }
    public decimal MeanDeviation => _compassCorrection.Mean;
    public ulong SampleCount => _compassCorrection.Count;


    public event PropertyChangedEventHandler PropertyChanged;

    private void TimerTick(object sender, EventArgs e)
    {
      CallPropertyChangedForStaleProperties();
    }

    private void OnUpdatePosition(QuantityWithMetadata<Coordinate> coordinate)
    {
      if (null == Sog || Sog.Source != SourceType.External || Sog.IsStale)
        // Try to calculate Sog from position data
        if (null != _lastposition && !_lastposition.IsStale)
        {
          var angulardistance = ((Coordinate)coordinate).Distance(_lastposition);
          var distance = Ball.EarthSurfaceApproximation.Distance(angulardistance).NauticalMiles();

          if (distance >= _minimumDistanceForCalculations.NauticalMiles())
          {
            var elapsed = DateTime.UtcNow - _lastposition.Updated;
            if (elapsed.HasValue && elapsed.Value >= _minimumElapsedtimeForCalculations)
            {
              Sog = distance / elapsed.Value.TotalHours;
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sog"));
            }
          }
        }

      if (null == Cog || Cog.Source != SourceType.External || Cog.IsStale)
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
              Cog = new TrueMessageCompassValue(truecog.Degrees);

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

    private static bool AnyStale(IEnumerable<IQuantityWithMetaData> properties)
    {
      return properties.Any(property => (null == property) || property.IsStale);
    }

    private void UpdateDeviation()
    {
      if (AnyStale(new IQuantityWithMetaData[] { Sog, Cog, Heading }))
        return;

      if (Sog < MINIMUM_SPEED_FOR_VALID_CALCULATIONS)
        return;

      _compassCorrection.AddSample(Heading.Value, Cog.Value);
      _compassCorrectionPersister.Write(_compassCorrection);

      CorrectedHeading = new QuantityWithMetadata<IMessageCompassValue>(_compassCorrection.CorrectHeading(Heading.Value));

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


    private void OnIncomingMessage(MessageBase message, DateTime messagetime)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (message.Name)
      {
        case MessageName.RMC:
          {
            var rmc = (RMC)message;
            if (rmc.SOG > MINIMUM_SPEED_FOR_VALID_CALCULATIONS)
            {
              Cog = rmc.TMG;
              Cog.Source = SourceType.External;
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cog"));
              Sog = rmc.SOG;
              Sog.Source = SourceType.External;
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sog"));
              OnUpdateCog();
            }
          }
          break;
        case MessageName.HDG:
          {
            var hdg = (HDG)message;
            if (null != hdg.MagneticContext)
              _magneticContext = hdg.MagneticContext;
          }
          break;
      }
      if (message is IHaveHeading)
      {
        Heading = new QuantityWithMetadata<IMessageCompassValue>(_magneticContext.Magnetic((message as IHaveHeading).Heading));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Heading"));
      }

      if (message is IHasPosition)
      {
        QuantityWithMetadata<Coordinate> c = (Coordinate)(message as IHasPosition).Position;
        c.Source = SourceType.External;
        c.Updated = messagetime;
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

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}