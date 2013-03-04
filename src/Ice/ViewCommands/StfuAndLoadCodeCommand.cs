using System;
using System.Windows.Input;

namespace CodeSaber.Ice.ViewCommands
{
  public class StfuAndLoadCodeCommand : ICommand
  {
    private CodeModel _ViewModel;

    public StfuAndLoadCodeCommand(CodeModel viewModel)
    {
      _ViewModel = viewModel;
    }


    public bool CanExecute(object parameter)
    {
      return false;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
      throw new NotImplementedException();
    }
  }
}
