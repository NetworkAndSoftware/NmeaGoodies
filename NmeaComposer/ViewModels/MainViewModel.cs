using System;
using System.ComponentModel;
using System.Dynamic;
using System.Net;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Nmea0183;
using NmeaComposer.ViewModels.Tabs;

namespace NmeaComposer.ViewModels
{
  internal class MainViewModel : INotifyPropertyChanged
  {
    private readonly MessageSender _messageSender;
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

    public string CompleteCommand
    {
      get { return null == _messageSender.Message ? null : _messageSender.Message.ToString(); }
    }

    public bool KeepSending
    {
      set
      {
        if (value)
        {
          _messageSender.Start();
        }
        else
        {
          _messageSender.Stop();
        }
      }
    }
 
    public MainViewModel()
    {
      Basic = new Basic(CreateMessageFromBasic);
      TalkerId = "SN";
      _messageSender = new MessageSender(IPAddress.Loopback);
    }

    public void CreateMessageFromBasic()
    {
      if (null != _messageSender)
      {
        if (null != TalkerId && null != Basic.CommandName && null != Basic.CommandBody)
          _messageSender.Message = new UnTypedMessage(TalkerId, Basic.CommandName, Basic.CommandBody);
        else
          _messageSender.Message = null;
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

      public event EventHandler CanExecuteChanged;
    }

    private void NotifyPropertyChanged(string propertyname)
    {
      if (PropertyChanged != null) 
        PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
