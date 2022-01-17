using System.Windows.Interop;
using Prism.Commands;
using Prism.Mvvm;
using SnapScreen.Library.Entities;
using SnapScreen.Library.Services;
using SnapScreen.Views;

namespace SnapScreen.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        private readonly IWinApiService winApiService;
        private readonly ISnapService snapService;

        private Layout layout;
        private SnapAreaTheme theme;

        public Layout Layout
        { get => layout; set { SetProperty(ref layout, value); } }

        public SnapAreaTheme Theme
        { get => theme; set { SetProperty(ref theme, value); } }

        public DesignWindow Window { get; set; }
        public Library.Entities.SnapScreen SnapScreen { get; set; }

        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand SaveLayoutCommand { get; }
        public DelegateCommand CloseLayoutCommand { get; }
        public DelegateCommand AddOverlayLayoutCommand { get; }
        public DelegateCommand ClearLayoutCommand { get; }

        public DesignWindowViewModel(
            IWinApiService winApiService,
            ISnapService snapService)
        {
            this.winApiService = winApiService;
            this.snapService = snapService;

            Theme = new SnapAreaTheme();
            Theme.ThemeChanged += Theme_ThemeChanged;

            LoadedCommand = new DelegateCommand(LoadedCommandExecute);
            SaveLayoutCommand = new DelegateCommand(SaveLayoutCommandExecute);
            CloseLayoutCommand = new DelegateCommand(CloseLayoutCommandExecute);
            AddOverlayLayoutCommand = new DelegateCommand(AddOverlayLayoutCommandExecute);
            ClearLayoutCommand = new DelegateCommand(ClearLayoutCommandExecute);
        }

        private void AddOverlayLayoutCommandExecute()
        {
            Window.SnapControl.AddOverlay();
        }

        private void ClearLayoutCommandExecute()
        {
            Window.SnapControl.ClearLayout();
        }

        private void LoadedCommandExecute()
        {
            //Window.Width = SnapScreen.WorkingArea.Width;
            //Window.Height = SnapScreen.WorkingArea.Height;
            //Window.Left = SnapScreen.WorkingArea.X;
            //Window.Top = SnapScreen.WorkingArea.Y;

            var wih = new WindowInteropHelper(Window);
            var activeWindow = new ActiveWindow
            {
                Handle = wih.Handle
            };

            winApiService.MoveWindow(activeWindow,
                                 (int)SnapScreen.WorkingArea.Left,
                                 (int)SnapScreen.WorkingArea.Top,
                                 (int)SnapScreen.WorkingArea.Width,
                                 (int)SnapScreen.WorkingArea.Height);

            snapService.Release();
        }

        private void SaveLayoutCommandExecute()
        {
            Window.SnapControl.Prepare(LayoutStatus.Saved);

            snapService.Initialize();

            Window.Close();
        }

        private void CloseLayoutCommandExecute()
        {
            Window.SnapControl.Prepare(LayoutStatus.Ignored);

            snapService.Initialize();

            Window.Close();
        }

        private void Theme_ThemeChanged()
        {
            Theme = Theme.Copy();
            Theme.ThemeChanged += Theme_ThemeChanged;
        }
    }
}