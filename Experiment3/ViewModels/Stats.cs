using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Threading;
using Experiment3.Annotations;
using Nmea0183.Communications;
using Nmea0183.Constants;
using Nmea0183.Messages;

namespace Experiment3.ViewModels
{
  internal class Stats : INotifyPropertyChanged
  {
    private const int MINIMUM_SPEED_FOR_VALID_CALCULATIONS = 1;
    private static readonly TimeSpan _updateStaleTime = TimeSpan.FromSeconds(3);

    public Stats()
    {
      MessageDispatcher.IncomingMessage += OnIncomingMessage;
      var timer = new DispatcherTimer() {Interval = TimeSpan.FromSeconds(1)};
      timer.Tick += TimerTick;
      timer.Start();
    }


    private bool IsStale(MessageName message) => !_lastMessageByType.ContainsKey(message) || DateTime.UtcNow - _lastMessageByType[message].Item2 > _updateStaleTime;




    private void TimerTick(object sender, EventArgs e)
    {
      var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(info => info.PropertyType == typeof(PropertyWithUpdateTime<double>));
      
      foreach (var info in properties)
      {
        var property = (PropertyWithUpdateTime<double>) info.GetValue(this);
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
            var hdm = (HDM)message;
            Heading = hdm.Heading;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Heading"));
          }
          break;

        case MessageName.RMC:
          {
            var rmc = (RMC)message;
            Cog = rmc.TMG;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("COG"));
          }
          break;

        

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

    public class PropertyWithUpdateTime<T>
    {
      private T _value;

      public T Value
      {
        get { return _value; }
        set { _value = value; LastUpdate = DateTime.Now;}
      }

      public bool IsStale => !LastUpdate.HasValue || DateTime.Now - LastUpdate > _updateStaleTime;

      public DateTime? LastUpdate { get; private set; }

      public bool UIToldItsStale { get; set; }

      public static implicit operator PropertyWithUpdateTime<T>(T value)
      {
        return new PropertyWithUpdateTime<T>() {Value = value};
      }

      public static implicit operator T(PropertyWithUpdateTime<T> data)
      {
        return data.Value;
      }
    }

    public PropertyWithUpdateTime<double> Heading { get; private set; }

    public PropertyWithUpdateTime<double> Cog { get; private set; }

    public PropertyWithUpdateTime<double> MeanDeviation { get; private set; }

    public PropertyWithUpdateTime<double> SampleCount { get; private set; }

    public PropertyWithUpdateTime<double> CorrectedHeading { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  internal class DataWithUpdateTimeConverter<T> : IValueConverter
  {
    public object Convert(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      var d = value as Stats.PropertyWithUpdateTime<T>;
      if (d == null)
      {
        return "(null)";
      }

      if (d.IsStale)
        return "Stale";

      return $"{d.Value:F3}";
    }

    public object ConvertBack(object value, Type targetType,
      object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class DoubleWithUpdateTimeConverter : DataWithUpdateTimeConverter<double>
  {
    
  }
}
