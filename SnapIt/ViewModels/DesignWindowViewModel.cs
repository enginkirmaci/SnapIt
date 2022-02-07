using System.Windows.Interop;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Views;

namespace SnapIt.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        private readonly IWinApiService winApiService;
        private readonly ISnapService snapService;

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

            Window.SnapControl.ResetBorderTool();
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