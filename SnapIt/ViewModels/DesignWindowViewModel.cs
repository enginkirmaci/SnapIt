using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Views;
using System.Windows.Media;

namespace SnapIt.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        private readonly ISnapService snapService;

        public SnapScreen SnapScreen { get; set; }
        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get; set; }
        public SnapArea MainSnapArea { get; set; }
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
                Opacity = 1
            };

            LoadedCommand = new DelegateCommand<object>((mainSnapArea) =>
            {
                snapService.Release();

                MainSnapArea = mainSnapArea as SnapArea;
                //TODO here
                //MainSnapArea.LayoutArea = Layout.LayoutArea;
                //MainSnapArea.Theme = Theme;

                //if (MainSnapArea.LayoutArea == null)
                //{
                //    MainSnapArea.LayoutArea = new LayoutArea();
                //}
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