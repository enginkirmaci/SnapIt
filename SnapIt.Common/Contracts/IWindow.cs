using System.Windows;

namespace SnapIt.Common.Contracts;

public interface IWindow
{
    event RoutedEventHandler Loaded;

    void Show();
}