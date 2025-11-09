using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;

namespace SnapIt.ViewModels.Pages;

public class WhatsNewPageViewModel : ViewModelBase
{
    private readonly List<ChangeLogItem> changeLogs =
    [
        new ChangeLogItem()
        {
            Header = "v5.0.7.0",
            Lines =
            [
                "Major refactor and cleanup.",
                "Migrated to .NET 9.0 and Microsoft DI.",
                "Simplified App startup and DI setup.",
                "Updated dependencies (Serilog, SharpHook, WPF-UI).",
                "Improved async file operations and error handling.",
                "Removed unused code, improved nullability."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v5.0.6.0",
            Lines =
                [
                    "Exclude app dialog fixes.",
                    "Hotkey fix"
                ]
        },
        new ChangeLogItem()
        {
            Header = "v5.0.5.0",
            Lines =
                [
                    "Some more performance improvements.",
                    "Color animations can be disabled on Settings page.",
                    "Replaced old hook with SharpHook for global mouse hooking."
                ]
        },
        new ChangeLogItem()
        {
            Header = "v5.0.0.0",
            Lines =
                [
                    "Entire codebase rewritten for better functionality and efficiency.",
                    "Made significant improvements to the quality of the code.",
                    "Resolved DPI and screen display problems.",
                    "Fixed several performance and user experience bugs.",
                    "Some functionalities have been removed (Apps and notify icon window)."
                ]
        },
        new ChangeLogItem()
        {
            Header = "v4.3.11.0",
            Lines =
            [
                "SnapIt is now open source and open to contribution"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v4.3.7.0",
            Lines = [
                "Run as administrator added to Microsoft Store version",
                "Screen viewer fix"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v4.3.5.0",
            Lines = [
                "Home and Tutorials added",
                "Improvements for quick action panel for Apps",
                "Added loading screen while Apps opening",
                "Minor improvements and fixes"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v4.2.0.0",
            Lines = [
                "Apps section added",
                "Improvements for UI",
            ]
        },
        new ChangeLogItem()
        {
            Header = "v4.0.9.0",
            Lines = [
                "Changed overlay activator and overlay designing, you can now freely move activator",
                "Added animation to areas and overlay when hovering",
                "Added designer option to space between areas",
                "Added border tool to set border X and Y coordinates"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v4.0.0.0",
            Lines = [
                "Windows 11 UI Design",
                "Minimize to tray at startup option added",
                "Improvement for screen detection and dpi handling",
                "Overall performance improvements",
                "Fixed layouts matching with different screen",
                "Fixed Enable/Disable screen for multiple screen"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.4.0.0",
            Lines = [
                "Fixed screen detection, will solve after sleep and plug/unplug monitor issues",
                "Minor UI fixes"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.3.8.0",
            Lines = [
                "Several minor bugfixes"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.3.6.0",
            Lines = [
                "Added missing Apply Changes to Theme",
                "UI Fixes for Light/Dark themes"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.3.4.0",
            Lines = [
                "Fixed Theme window causing not navigating menu.",
                "Improved error handling.",
                "Website links updated.",
                "Other minor fixes."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.2.8.0",
            Lines = [
                "New UI, follows Windows 10 fluent design.",
                "Disabling snapping for popup/modal windows can be disabled from windows setting.",
                "Hex code can be used to set color."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.1.6.0",
            Lines = [
                "Fixed layout export."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.1.5.0",
            Lines = [
                "Reimplemented keyboard snapping.",
                "Added shortcut key for Start/Stop.",
                "Added override button for overriding windows default snap shortcuts (Win+Arrow).",
                "Added option to enable/disable snapping for monitors.",
                "Disabled snapping for application's popup/modal windows.",
                "Fixed DPI and connect/disconnect monitor.",
            ]
        },
        new ChangeLogItem()
        {
            Header = "v3.0.0.0",
            Lines = [
                "Redesigned layout mechanism.",
                "Improved layout designer.",
                "Fixes for layout save, rename and cancel functionalities."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v2.0.6.0",
            Lines = [
                "Fixed adding/editing layout causing errors.",
                "Improved keyboard snapping."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v2.0.2.0",
            Lines = [
                "Added ability to change layout from tray icon.",
                "Added new shortcut for cycling throught layouts for active screen."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v2.0.0.0",
            Lines = [
                "Redesigned overlay window and controls",
                "New UI",
                "Theme added so overlay window UI elements can be customized.",
                "Minor bugfixes and improvements."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.4.1.0",
            Lines = [
                "Improvements for dpi, resolution and screen changes.",
                "Added change log to about.",
                "Fix for Excluded application wildcard doesn't work.",
                "Merged snap area boxes excluded from keyboard snapping.",
                "Minor bugfixes."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.3.6.0",
            Lines = [
                "Developed new Window screen and functionality.",
                "Improvements for better ESC key handling while snapping.",
                "Minor bugfixes."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.3.3.0",
            Lines = [
                "Improvements for assigning HotKey for keyboard snapping.",
                "Improvements for hold key behaviour.",
                "Improvements for Left click snapping.",
                "Some other minor changes."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.2.9.0",
            Lines = [
                "Added Delay time for mouse snapping.",
                "Added Hold key for mouse snapping.",
                "Added detection of screen Dpi changes."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.2.5.0",
            Lines = [
                "Merged snapping area added.",
                "Layout editor changes to support merged snapping area.",
                "Lot of bugfixes and performance improvements."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.1.0.0",
            Lines = [
                "Maximized window handling developed for snapping.",
                "Feedback and support links/buttons added."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.0.8.0",
            Lines = [
                "Dpi aware snapping developed",
                "Light theme added",
                "Deleting layout option added.",
                "Added context menu to Layout screen"
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.0.4.0",
            Lines = [
                "Minor UI changes",
                "Added window and excluded applications",
                "Added keyboard snapping with hotkey."
            ]
        },
        new ChangeLogItem()
        {
            Header = "v1.0.0.0",
            Lines = [
                "First Release"
            ]
        }
    ];

    public IEnumerable<ChangeLogItem> ChangeLogs => changeLogs.Skip(1);

    public ChangeLogItem FirstChangeLog => changeLogs.FirstOrDefault();

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
    }
}