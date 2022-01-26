using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SnapScreen.Test.DesignTime
{
    public enum Themes
    {
        Light,
        Dark,
        System
    }

    public class MainWindowDesignViewModel
    {
        public string ThemeTitle { get; set; }
        public bool IsDarkTheme { get; set; }
        public bool IsRunning { get; set; }
        public string Status { get; set; }
        public bool IsVersion3000MessageShown { get; set; }
        public ObservableCollection<Themes> ThemeList { get; set; }
        public Themes SelectedTheme { get; set; }
        public ICommand ThemeItemCommand { get; }

        public MainWindowDesignViewModel()
        {
            IsRunning = true;
            Status = "Stop";
            ThemeTitle = "Light";
            IsDarkTheme = false;
            IsVersion3000MessageShown = false;

            ThemeList = new ObservableCollection<Themes> {
                Themes.Light,
                Themes.Dark,
                Themes.System
            };

            SelectedTheme = Themes.System;

            this.ThemeItemCommand = new SimpleCommand(
               o => true,
               x => { SelectedTheme = (Themes)x; }
           );
        }
    }

    public class SimpleCommand : ICommand
    {
        public SimpleCommand(Func<object, bool> canExecute = null, Action<object> execute = null)
        {
            this.CanExecuteDelegate = canExecute;
            this.ExecuteDelegate = execute;
        }

        public Func<object, bool> CanExecuteDelegate { get; set; }

        public Action<object> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            var canExecute = this.CanExecuteDelegate;
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            this.ExecuteDelegate?.Invoke(parameter);
        }
    }
}