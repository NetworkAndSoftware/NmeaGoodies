using System;
using System.Windows;

namespace Experiment3.Helpers
{
  internal class WindowCommand : DelegateCommand<Window>
  {
    public WindowCommand(Action<Window> execute) : base(execute)
    {
    }

    public WindowCommand(Action<Window> execute, Predicate<Window> canExecute) : base(execute, canExecute)
    {
    }
  }
}