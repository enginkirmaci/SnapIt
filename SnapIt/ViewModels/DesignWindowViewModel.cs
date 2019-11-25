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
        public SnapArea MainSnapArea { get; set; }
        public DesignWindow Window { get; set; }

        //public DelegateCommand<Window> SourceInitializedCommand { get; }
        public DelegateCommand<object> LoadedCommand { get; }

        public DelegateCommand SaveLayoutCommand { get; }
        public DelegateCommand CloseLayoutCommand { get; }

        public DesignWindowViewModel(ISnapService snapService)
        {
            this.snapService = snapService;

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

                MainSnapArea = mainSnapArea as SnapArea;

                MainSnapArea.ApplyLayout(Layout.LayoutArea, true);
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