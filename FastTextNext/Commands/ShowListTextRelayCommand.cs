using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastTextNext.Commands
{
    public class ShowListTextRelayCommand : ICommand
    {
        private Action? onOpenSettingsDialog;
        private readonly Func<object, bool> canExecute;

        public ShowListTextRelayCommand(Action? onOpenSettingsDialog, Func<object, bool> canExecute = null)
        {
            this.onOpenSettingsDialog = onOpenSettingsDialog;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            var result = canExecute == null || canExecute(parameter);
            return result;
        }

        public void Execute(object? parameter)
        {
            onOpenSettingsDialog?.Invoke();
        }
    }
}
