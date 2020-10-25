using System;
using System.Windows.Input;

namespace Imperialism_2_SGEditor
{
  public class DelegateCommandBase : ICommand
  {
    private readonly Action<object> executeMethod;
    private readonly Func<object, bool> canExecuteMethod;

    public event EventHandler CanExecuteChanged;

    public DelegateCommandBase(Action<object> execute)
                   : this(execute, null)
    {
    }

    public DelegateCommandBase(Action<object> execute, Func<object, bool> canExecute)
    {
      executeMethod = execute;
      canExecuteMethod = canExecute;

      if (executeMethod == null || canExecuteMethod == null)
        throw new ArgumentNullException("DelegateCommand delegates cannot be null");
    }

    void ICommand.Execute(object parameter)
    {
      Execute(parameter);
    }

    bool ICommand.CanExecute(object parameter)
    {
      return CanExecute(parameter);
    }

    protected void Execute(object parameter)
    {
      executeMethod(parameter);
    }

    protected bool CanExecute(object parameter)
    {
      return canExecuteMethod == null || canExecuteMethod(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  public class DelegateCommand : DelegateCommandBase
  {
    public DelegateCommand(Action executeMethod) : this(executeMethod, () => true)
    {
    }

    public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        : base((o) => executeMethod(), (o) => canExecuteMethod())
    {
      if (executeMethod == null || canExecuteMethod == null)
        throw new ArgumentNullException("DelegateCommand delegates cannot be null");
    }

    public void Execute()
    {
      Execute(null);
    }

    public bool CanExecute()
    {
      return CanExecute(null);
    }
  }

  public class DelegateCommand<T> : DelegateCommandBase
  {
    public DelegateCommand(Action<T> executeMethod)
        : this(executeMethod, (o) => true)
    {
    }

    public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        : base((o) => executeMethod((T)o), (o) => canExecuteMethod((T)o))
    {
      if (executeMethod == null || canExecuteMethod == null)
        throw new ArgumentNullException("DelegateCommand delegates cannot be null");
    }

    public bool CanExecute(T parameter)
    {
      return base.CanExecute(parameter);
    }

    public void Execute(T parameter)
    {
      base.Execute(parameter);
    }
  }
}
