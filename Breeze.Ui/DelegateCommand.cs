using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Breeze.Ui
{
        public delegate bool CanExcute(object parameter);
    public delegate void Excute(object parameter);

    public class DelegateCommand : ICommand
    {
        private Excute executeDelegate;
        private CanExcute canExecuteDelegate;

        public DelegateCommand(Excute executeDelegate) : this(executeDelegate, null) { }

        public DelegateCommand(Excute executeDelegate, CanExcute canExecuteDelegate)
        {
            this.executeDelegate = executeDelegate;
            this.canExecuteDelegate = canExecuteDelegate;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            var temp = canExecuteDelegate;

            if (temp != null)
                return temp(parameter);

            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var temp = executeDelegate;

            if (temp != null)
                temp(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            var temp = CanExecuteChanged;

            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        #endregion
    }
}
