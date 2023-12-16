using System.Windows;
using Prism.Commands;
using SnapIt.Common.Converters;
using SnapIt.Common.Mvvm;
using Wpf.Ui;

namespace SnapIt.ViewModels.Pages;

public class DashboardPageViewModel : ViewModelBase
{
    public DelegateCommand<string> NavigateCommand { get; private set; }

    public DashboardPageViewModel(INavigationService navigationService)
    {
        NavigateCommand = new DelegateCommand<string>((parameter) =>
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return;
            }

            Type? pageType = NameToPageTypeConverter.Convert(parameter);

            if (pageType == null)
            {
                return;
            }

            navigationService.Navigate(pageType);
        });
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
    }
}