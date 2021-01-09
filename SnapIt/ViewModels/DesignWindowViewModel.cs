using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Views;

namespace SnapIt.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private Layout layout;

        public Layout Layout { get => layout; set => SetProperty(ref layout, value); }

        public SnapScreen SnapScreen { get; set; }
        public SnapAreaTheme Theme { get; set; }
        public SnapControl SnapControl { get; set; }
        public DesignWindow Window { get; set; }

        //public DelegateCommand<Window> SourceInitializedCommand { get; }
        public DelegateCommand<object> LoadedCommand { get; }

        public DelegateCommand<object> ClosingCommand { get; }

        public DelegateCommand SaveLayoutCommand { get; }
        public DelegateCommand CloseLayoutCommand { get; }

        public DesignWindowViewModel(ISnapService snapService)
        {
            this.snapService = snapService;

            Theme = new SnapAreaTheme();

            LoadedCommand = new DelegateCommand<object>((snapControl) =>
            {
                snapService.Release();

                SnapControl = snapControl as SnapControl;
            });

            ClosingCommand = new DelegateCommand<object>((snapControl) =>
            {
                SnapControl = snapControl as SnapControl;
                SnapControl.SetLayoutSize();
            });

            SaveLayoutCommand = new DelegateCommand(() =>
            {
                snapService.Initialize();

                Window.Close();
            });

            CloseLayoutCommand = new DelegateCommand(() =>
            {
                Window.Close();
            });
        }
    }
}