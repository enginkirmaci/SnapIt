using System.Collections.Generic;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class ThemeViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private SnapAreaTheme theme;

        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get => settingService.Settings.Theme; set { SetProperty(ref theme, value); settingService.Settings.Theme = value; } }

        public ThemeViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            Layout = new Layout
            {
                Name = "Layout 1",
                LayoutArea = new LayoutArea
                {
                    Areas = new List<LayoutArea>
                    {
                        new LayoutArea
                        {
                            Width=1
                        },
                        new LayoutArea
                        {
                            Width=3,
                            Column=1,
                            Merged= true,
                            Areas = new List<LayoutArea>
                            {
                                new LayoutArea
                                {
                                    Height=1
                                },
                                new LayoutArea
                                {
                                    Height=1,
                                    Row=1,
                                    Areas = new List<LayoutArea>
                                    {
                                        new LayoutArea
                                        {
                                            Width=1
                                        },
                                        new LayoutArea
                                        {
                                            Width=1,
                                            Column=1
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private void ApplyChanges()
        {
            if (!DevMode.IsActive)
            {
                snapService.Release();
                snapService.Initialize();
            }
        }
    }
}