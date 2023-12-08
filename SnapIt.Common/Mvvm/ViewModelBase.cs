using System.Windows;
using Prism.Commands;

namespace SnapIt.Common.Mvvm;

public abstract class ViewModelBase : Bindable
{
    public DelegateCommand<RoutedEventArgs> LoadedCommand { get; private set; }

    public ViewModelBase()
    {
        LoadedCommand = new DelegateCommand<RoutedEventArgs>(OnLoaded);
    }

    public virtual void OnLoaded(RoutedEventArgs args)
    {
        InitializeAsync();
    }

    public abstract Task InitializeAsync();
}