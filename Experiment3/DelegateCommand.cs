using System;

namespace Experiment3
{
  internal class DelegateCommand<T> : System.Windows.Input.ICommand where T : class
  {
    private readonly Predicate<T> _canExecute;
    private readonly Action<T> _execute;

    public DelegateCommand(Action<T> execute)
      : this(execute, null)
    {
    }

    public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
    {
      _execute = execute;
      _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
      if (_canExecute == null)
        return true;

      return _canExecute((T)parameter);
    }

    public void Execute(object parameter)
    {
      _execute((T)parameter);
    }

    public event EventHandler CanExecuteChanged;
    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }


  internal class DelegateCommand : System.Windows.Input.ICommand
  {
    private readonly Action _execute;

    public DelegateCommand(Action execute)
    {
      _execute = execute;
    }

    public bool CanExecute(object o)
    {
      return true;
    }

    public void Execute(object o)
    {
      _execute();
    }

    public event EventHandler CanExecuteChanged;
  }
}