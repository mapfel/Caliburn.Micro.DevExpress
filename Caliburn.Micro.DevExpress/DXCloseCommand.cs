using System;
using System.Windows.Input;

namespace Caliburn.Micro.DevExpress
{
  /// <summary>
  /// Command type to support closing documentpanels.
  /// </summary>
  public class DXCloseCommand : ICommand
  {
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public DXCloseCommand(Action<object> execute)
      : this(execute, null)
    {
    }

    public DXCloseCommand(Action<object> execute, Predicate<object> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");
      _execute = execute;
      _canExecute = canExecute;
    }

    #region ICommand Members

    public bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    #endregion ICommand Members
  }
}