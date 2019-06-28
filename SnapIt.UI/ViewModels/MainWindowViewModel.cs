using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Services;

namespace SnapIt.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ISnapService snapService;

        public string Title { get; set; } = $"Snap It {System.Windows.Forms.Application.ProductVersion}";

        public DelegateCommand<Window> CloseWindowCommand { get; private set; }

        public MainWindowViewModel(
            ISnapService snapService)
        {
            this.snapService = snapService;

            snapService.Initialize();

            CloseWindowCommand = new DelegateCommand<Window>((window) =>
            {
                if (window != null)
                {
                    window.Hide();
                }
            });
        }
    }
}