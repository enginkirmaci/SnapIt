using System.Windows.Interop;
using Prism.Commands;
using SnapIt.Application.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;
using SnapIt.Services.Contracts;
using SnapIt.Views.Windows;

namespace SnapIt.ViewModels.Windows
{
    public class DesignWindowViewModel : ViewModelBase
    {
        private readonly IWinApiService winApiService;
        private readonly ISnapManager snapManager;

        //private bool isOverlayVisible ;
        private Layout layout;

        private SnapAreaTheme theme;

        public bool IsOverlayVisible
        { get { return Window.SnapControl.IsOverlayVisible; } set { Window.SnapControl.IsOverlayVisible = value; } }

        //public int AreaPadding
        //{ get { return Window != null ? Window.SnapControl.AreaPadding : 0; } set { Window.SnapControl.AreaPadding = value; } }

        public Layout Layout
        { get => layout; set { SetProperty(ref layout, value); } }

        public SnapAreaTheme Theme
        { get => theme; set { SetProperty(ref theme, value); } }

        public DesignWindow Window { get; set; }
        public SnapScreen SnapScreen { get; set; }

        public DelegateCommand SaveLayoutCommand { get; }
        public DelegateCommand CloseLayoutCommand { get; }
        public DelegateCommand AddOverlayLayoutCommand { get; }
        public DelegateCommand ClearLayoutCommand { get; }

        public DesignWindowViewModel(
            IWinApiService winApiService,
            ISnapManager snapManager)
        {
            this.winApiService = winApiService;
            this.snapManager = snapManager;

            Theme = new SnapAreaTheme();
            Theme.ThemeChanged += Theme_ThemeChanged;

            SaveLayoutCommand = new DelegateCommand(SaveLayoutCommandExecute);
            CloseLayoutCommand = new DelegateCommand(CloseLayoutCommandExecute);
            AddOverlayLayoutCommand = new DelegateCommand(AddOverlayLayoutCommandExecute);
            ClearLayoutCommand = new DelegateCommand(ClearLayoutCommandExecute);
        }

        public override async Task InitializeAsync()
        {
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

            snapManager.Release();

            Window.SnapControl.ResetBorderTool();
        }

        private void AddOverlayLayoutCommandExecute()
        {
            Window.SnapControl.AddOverlay();
        }

        private void ClearLayoutCommandExecute()
        {
            Window.SnapControl.ClearLayout();
        }

        private void SaveLayoutCommandExecute()
        {
            Window.SnapControl.Prepare(LayoutStatus.Saved);

            _ = snapManager.InitializeAsync();

            Window.Close();
        }

        private void CloseLayoutCommandExecute()
        {
            Window.SnapControl.Prepare(LayoutStatus.Ignored);

            _ = snapManager.InitializeAsync();

            Window.Close();
        }

        private void Theme_ThemeChanged()
        {
            Theme = Theme.Copy();
            Theme.ThemeChanged += Theme_ThemeChanged;
        }
    }
}