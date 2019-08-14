using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Views;

namespace SnapIt.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        public SnapScreen SnapScreen { get; set; }
        public Layout Layout { get; set; }
        public SnapArea MainSnapArea { get; set; }
        public DesignWindow Window { get; set; }

        //public DelegateCommand<Window> SourceInitializedCommand { get; }
        public DelegateCommand<object> LoadedCommand { get; }

        public DelegateCommand SaveLayoutCommand { get; }

        public DesignWindowViewModel()
        {
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
                MainSnapArea = mainSnapArea as SnapArea;

                MainSnapArea.ApplyLayout(Layout.LayoutArea, true);
            });

            SaveLayoutCommand = new DelegateCommand(() =>
            {
                Layout.GenerateLayoutArea(MainSnapArea);

                Window.Close();
            });
        }
    }
}