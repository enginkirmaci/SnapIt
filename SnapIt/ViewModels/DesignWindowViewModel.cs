using System.Windows.Media;
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

        public SnapScreen SnapScreen { get; set; }
        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get; set; }
        public SnapAreaNew MainSnapArea { get; set; }
        public DesignWindow Window { get; set; }

        //public DelegateCommand<Window> SourceInitializedCommand { get; }
        public DelegateCommand<object> LoadedCommand { get; }

        public DelegateCommand SaveLayoutCommand { get; }
        public DelegateCommand CloseLayoutCommand { get; }

        public DesignWindowViewModel(ISnapService snapService)
        {
            this.snapService = snapService;

            Theme = new SnapAreaTheme
            {
                HighlightColor = Color.FromArgb(200, 33, 33, 33),
                OverlayColor = Color.FromArgb(50, 99, 99, 99),
                BorderColor = Color.FromArgb(200, 200, 200, 200),
                //BorderThickness = 1,
                Opacity = 1
            };

            //SourceInitializedCommand = new DelegateCommand<Window>((window) =>
            //{
            //	var wih = new WindowInteropHelper(window);
            //	var activeWindow = new ActiveWindow
            //	{
            //		Handle = wih.Handle
            //	};

            //	User32Test.MoveWindow(activeWindow,
            //						  SnapScreen.Base.WorkingArea.Left,
            //						  SnapScreen.Base.WorkingArea.Top,
            //						  SnapScreen.Base.WorkingArea.Width,
            //						  SnapScreen.Base.WorkingArea.Height);
            //});

            LoadedCommand = new DelegateCommand<object>((mainSnapArea) =>
            {
                snapService.Release();

                MainSnapArea = mainSnapArea as SnapAreaNew;
                MainSnapArea.LayoutArea = Layout.LayoutArea;
                MainSnapArea.Theme = Theme;
                //MainSnapArea.IsDesignMode = true;

                //MainSnapArea.ApplyLayout(Layout.LayoutArea, true);
            });

            SaveLayoutCommand = new DelegateCommand(() =>
            {
                snapService.Initialize();

                Layout.GenerateLayoutArea(MainSnapArea);

                Window.Close();
            });

            CloseLayoutCommand = new DelegateCommand(() =>
            {
                Window.Close();
            });
        }
    }
}