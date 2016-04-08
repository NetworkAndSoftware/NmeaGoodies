using System;
using System.Data.SqlClient;

namespace NmeaComposer.ViewModels.Tabs
{
  class Basic
  {
    private readonly Action _onChange;
    private string _commandName;
    private string _commandBody;

    public Basic(Action onChange)
    {
      _onChange = onChange;
    }

    public string CommandName
    {
      get { return _commandName; }
      set { _commandName = value; _onChange.Invoke();}
    }

    public string CommandBody
    {
      get { return _commandBody; }
      set { _commandBody = value; _onChange.Invoke(); }
    }
  }
}
