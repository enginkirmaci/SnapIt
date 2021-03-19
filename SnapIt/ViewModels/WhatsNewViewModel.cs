using System.Collections.Generic;
using Prism.Mvvm;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels
{
    public class WhatsNewViewModel : BindableBase
    {
        public List<ChangeLogItem> ChangeLogs
        {
            get
            {
                return new List<ChangeLogItem>
                {
                    new ChangeLogItem()
                    {
                        Header = "v3.3.6.0",
                        Lines = new List<string>() {
                            "Added missing Apply Changes to Theme",
                            "UI Fixes for Light/Dark themes"
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v3.3.4.0",
                        Lines = new List<string>() {
                            "Fixed Theme window causing not navigating menu.",
                            "Improved error handling.",
                            "Website links updated.",
                            "Other minor fixes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v3.2.8.0",
                        Lines = new List<string>() {
                            "New UI, follows Windows 10 fluent design.",
                            "Disabling snapping for popup/modal windows can be disabled from windows setting.",
                            "Hex code can be used to set color."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v3.1.6.0",
                        Lines = new List<string>() {
                            "Fixed layout export."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v3.1.5.0",
                        Lines = new List<string>() {
                            "Reimplemented keyboard snapping.",
                            "Added shortcut key for Start/Stop.",
                            "Added override button for overriding windows default snap shortcuts (Win+Arrow).",
                            "Added option to enable/disable snapping for monitors.",
                            "Disabled snapping for application's popup/modal windows.",
                            "Fixed DPI and connect/disconnect monitor.",
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v3.0.0.0",
                        Lines = new List<string>() {
                            "Redesigned layout mechanism.",
                            "Improved layout designer.",
                            "Fixes for layout save, rename and cancel functionalities."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v2.0.6.0",
                        Lines = new List<string>() {
                            "Fixed adding/editing layout causing errors.",
                            "Improved keyboard snapping."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v2.0.2.0",
                        Lines = new List<string>() {
                            "Added ability to change layout from tray icon.",
                            "Added new shortcut for cycling throught layouts for active screen."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v2.0.0.0",
                        Lines = new List<string>() {
                            "Redesigned overlay window and controls",
                            "New UI",
                            "Theme added so overlay window UI elements can be customized.",
                            "Minor bugfixes and improvements."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.4.1.0",
                        Lines = new List<string>() {
                            "Improvements for dpi, resolution and screen changes.",
                            "Added change log to about.",
                            "Fix for Excluded application wildcard doesn't work.",
                            "Merged snap area boxes excluded from keyboard snapping.",
                            "Minor bugfixes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.3.6.0",
                        Lines = new List<string>() {
                            "Developed new Window screen and functionality.",
                            "Improvements for better ESC key handling while snapping.",
                            "Minor bugfixes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.3.3.0",
                        Lines = new List<string>() {
                            "Improvements for assigning HotKey for keyboard snapping.",
                            "Improvements for hold key behaviour.",
                            "Improvements for Left click snapping.",
                            "Some other minor changes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.2.9.0",
                        Lines = new List<string>() {
                            "Added Delay time for mouse snapping.",
                            "Added Hold key for mouse snapping.",
                            "Added detection of screen Dpi changes."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.2.5.0",
                        Lines = new List<string>() {
                            "Merged snapping area added.",
                            "Layout editor changes to support merged snapping area.",
                            "Lot of bugfixes and performance improvements."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.1.0.0",
                        Lines = new List<string>() {
                            "Maximized window handling developed for snapping.",
                            "Feedback and support links/buttons added."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.0.8.0",
                        Lines = new List<string>() {
                            "Dpi aware snapping developed",
                            "Light theme added",
                            "Deleting layout option added.",
                            "Added context menu to Layout screen"
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.0.4.0",
                        Lines = new List<string>() {
                            "Minor UI changes",
                            "Added window and excluded applications",
                            "Added keyboard snapping with hotkey."
                        }
                    },
                    new ChangeLogItem()
                    {
                        Header = "v1.0.0.0",
                        Lines = new List<string>() {
                            "First Release"
                        }
                    }
                };
            }
        }
    }
}