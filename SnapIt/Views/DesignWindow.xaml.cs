using System;
using System.Windows;
using System.Windows.Interop;
using SnapIt.Library;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.ViewModels;

namespace SnapIt.Views
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : Window
    {
        private readonly IWinApiService winApiService;
        private SnapScreen snapScreen;

        public DesignWindow(IWinApiService winApiService)
        {
            InitializeComponent();

            this.winApiService = winApiService;
        }

        public void SetScreen(SnapScreen snapScreen, Layout layout)
        {
            this.snapScreen = snapScreen;

            Width = snapScreen.Base.WorkingArea.Width;
            Height = snapScreen.Base.WorkingArea.Height;
            Left = snapScreen.Base.WorkingArea.X;
            Top = snapScreen.Base.WorkingArea.Y;

            var model = DataContext as DesignWindowViewModel;
            model.Window = this;
            model.SnapScreen = snapScreen;
            model.Layout = layout;

            if (DevMode.IsActive)
            {
                Topmost = false;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var wih = new WindowInteropHelper(this);
            var window = new ActiveWindow
            {
                Handle = wih.Handle
            };

            winApiService.MoveWindow(window,
                                  snapScreen.Base.WorkingArea.Left,
                                  snapScreen.Base.WorkingArea.Top,
                                  snapScreen.Base.WorkingArea.Width,
                                  snapScreen.Base.WorkingArea.Height);
        }
    }
}