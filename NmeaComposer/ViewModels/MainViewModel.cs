using System;
using System.ComponentModel;
using System.Windows.Input;
using Nmea0183;
using Nmea0183.Communications;
using Nmea0183.Messages;
using NmeaComposer.ViewModels.Tabs;

namespace NmeaComposer.ViewModels
{
  internal class MainViewModel : INotifyPropertyChanged
  {
    private readonly RepeatingSender _repeatingSender;
    private string _talkerId;

    public string TalkerId
    {
      get { return _talkerId; }
      set
      {
        _talkerId = value; 
        CreateMessageFromBasic(); 
      }
    }

    public Basic Basic { get; set; }

    public string CompleteCommand => _repeatingSender.Message?.ToString();

    public bool KeepSending
    {
      set
      {
        if (value)
        {
          _repeatingSender.Start();
        }
        else
        {
          _repeatingSender.Stop();
        }
      }
    }
 
    public MainViewModel()
    {
      Basic = new Basic(CreateMessageFromBasic);
      TalkerId = "SN";
      _repeatingSender = new RepeatingSender(TimeSpan.FromSeconds(1));
    }

    public void CreateMessageFromBasic()
    {
      if (null != _repeatingSender)
      {
        if (null != TalkerId && null != Basic.CommandName && null != Basic.CommandBody)
          _repeatingSender.Message = new UnknownMessage(TalkerId, Basic.CommandName, Basic.CommandBody);
        else
          _repeatingSender.Message = null;
      }


      NotifyPropertyChanged("CompleteCommand");
    }

    public class CreateMessageFromBasicCommandClass : ICommand
    {
      private readonly MainViewModel _viewmodel; 
      public CreateMessageFromBasicCommandClass(MainViewModel viewmodel) 
      {
        _viewmodel = viewmodel;
      }
      public bool CanExecute(object parameter)
      {
        return true; 
      }
      public void Execute(object parameter)
      {
        _viewmodel.CreateMessageFromBasic(); 
      }


      public event EventHandler CanExecuteChanged { add { } remove { } }
    }

    private void NotifyPropertyChanged(string propertyname)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
    }

    public event PropertyChangedEventHandler PropertyChanged;

  }
}
