using System.Windows.Interop;
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
        private readonly IWinApiService winApiService;
        private readonly ISnapService snapService;

        private Layout layout;
        private string _currentName;

        public Layout Layout
        {
            get => layout; set
            {
                SetProperty(ref layout, value);
                _currentName = layout.Name;
            }
        }

        public DesignWindow Window { get; set; }
        public SnapScreen SnapScreen { get; set; }
        public SnapAreaTheme Theme { get; set; }

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
            Window.Width = SnapScreen.Base.WorkingArea.Width;
            Window.Height = SnapScreen.Base.WorkingArea.Height;
            Window.Left = SnapScreen.Base.WorkingArea.X;
            Window.Top = SnapScreen.Base.WorkingArea.Y;

            var wih = new WindowInteropHelper(Window);
            var activeWindow = new ActiveWindow
            {
                Handle = wih.Handle
            };

            winApiService.MoveWindow(activeWindow,
                                  SnapScreen.Base.WorkingArea.Left,
                                  SnapScreen.Base.WorkingArea.Top,
                                  SnapScreen.Base.WorkingArea.Width,
                                  SnapScreen.Base.WorkingArea.Height);

            snapService.Release();
        }

        private void SaveLayoutCommandExecute()
        {
            Window.SnapControl.SetLayoutSize();
            Layout.IsSaved = false;
            snapService.Initialize();

            Window.Close();
        }

        private void CloseLayoutCommandExecute()
        {
            Window.SnapControl.SetLayoutSize();
            Layout.Name = _currentName;
            Layout.IsSaved = true;

            Window.Close();
        }
    }
}