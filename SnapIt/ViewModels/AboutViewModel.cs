using System;
using System.Collections.Generic;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        private bool isChangeLogDialogOpen;

        public DelegateCommand<string> HandleLinkClick { get; private set; }
        public DelegateCommand RateReviewStoreClick { get; private set; }
        public DelegateCommand OpenChangeLogDialog { get; private set; }
        public bool IsChangeLogDialogOpen { get => isChangeLogDialogOpen; set => SetProperty(ref isChangeLogDialogOpen, value); }

        public AboutViewModel()
        {
            RateReviewStoreClick = new DelegateCommand(async () =>
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Constants.AppStoreId}"));
            });

            HandleLinkClick = new DelegateCommand<string>(async (url) =>
            {
                string uriToLaunch = $"http://{url}";
                var uri = new Uri(uriToLaunch);

                await Windows.System.Launcher.LaunchUriAsync(uri);
            });

            OpenChangeLogDialog = new DelegateCommand(() =>
             {
                 IsChangeLogDialogOpen = true;
             });
        }

        public List<ChangeLogItem> ChangeLogs
        {
            get
            {
                return new List<ChangeLogItem>
                {
                    new ChangeLogItem()
                    {
                        Header = "v2.0.2.0",
                        Lines = new List<string>() {
                            "- Added ability to change layout from tray icon.",
                            "- Added new shortcut for cycling throught layouts for active screen."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v2.0.0.0",
                        Lines = new List<string>() {
                            "- Redesigned overlay window and controls",
                            "- New UI",
                            "- Theme added so overlay window UI elements can be customized.",
                            "- Minor bugfixes and improvements."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.4.1.0",
                        Lines = new List<string>() {
                            "- Improvements for dpi, resolution and screen changes.",
                            "- Added change log to about.",
                            "- Fix for Excluded application wildcard doesn't work.",
                            "- Merged snap area boxes excluded from keyboard snapping.",
                            "- Minor bugfixes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.3.6.0",
                        Lines = new List<string>() {
                            "- Developed new Window screen and functionality.",
                            "- Improvements for better ESC key handling while snapping.",
                            "- Minor bugfixes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.3.3.0",
                        Lines = new List<string>() {
                            "- Improvements for assigning HotKey for keyboard snapping.",
                            "- Improvements for hold key behaviour.",
                            "- Improvements for Left click snapping.",
                            "- Some other minor changes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.2.9.0",
                        Lines = new List<string>() {
                            "- Added Delay time for mouse snapping.",
                            "- Added Hold key for mouse snapping.",
                            "- Added detection of screen Dpi changes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.2.5.0",
                        Lines = new List<string>() {
                            "- Merged snapping area added.",
                            "- Layout editor changes to support merged snapping area.",
                            "- Lot of bugfixes and performance improvements."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.1.0.0",
                        Lines = new List<string>() {
                            "- Maximized window handling developed for snapping.",
                            "- Feedback and support links/buttons added."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.0.8.0",
                        Lines = new List<string>() {
                            "- Dpi aware snapping developed",
                            "- Light theme added",
                            "- Deleting layout option added.",
                            "- Added context menu to Layout screen"
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.0.4.0",
                        Lines = new List<string>() {
                            "- Minor UI changes",
                            "- Added window and excluded applications",
                            "- Added keyboard snapping with hotkey."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.0.0.0",
                        Lines = new List<string>() {
                            "- First Release"
                        }
                    }
                };
            }
        }
    }
}