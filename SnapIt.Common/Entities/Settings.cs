namespace SnapIt.Common.Entities;

public class Settings
{
    public Settings()
    {
        ScreensLayouts = [];
        DeactivedScreens = [];
    }

    public string Version = "2.0";
    public bool EnableMouse { get; set; } = true;
    public bool DragByTitle { get; set; } = true;
    public MouseButton MouseButton { get; set; } = MouseButton.Left;
    public int MouseDragDelay { get; set; } = 20;
    public bool EnableHoldKey { get; set; } = false;
    public HoldKey HoldKey { get; set; } = HoldKey.Control;
    public HoldKeyBehaviour HoldKeyBehaviour { get; set; } = HoldKeyBehaviour.HoldToEnable;
    public bool EnableKeyboard { get; set; } = true;
    public bool DisableForFullscreen { get; set; } = true;
    public bool DisableForModal { get; set; } = true;
    public bool ShowMainWindow { get; set; } = true;
    public UITheme AppTheme { get; set; } = UITheme.System;
    public Dictionary<string, string> ScreensLayouts { get; set; }
    public List<string> DeactivedScreens { get; set; }
    public string MoveUpShortcut { get; set; } = "Ctrl + Alt + Up";
    public string MoveDownShortcut { get; set; } = "Ctrl + Alt + Down";
    public string MoveLeftShortcut { get; set; } = "Ctrl + Alt + Left";
    public string MoveRightShortcut { get; set; } = "Ctrl + Alt + Right";
    public string CycleLayoutsShortcut { get; set; } = "Ctrl + Alt + C";
    public string StartStopShortcut { get; set; } = "Ctrl + Alt + S";
    public bool IsVersion3000MessageShown { get; set; }
    public bool CheckForNewVersion { get; set; } = true;

    //public Color HighlightColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 33, 33, 33).Convert();
    //public Color OverlayColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 99, 99, 99).Convert();
    //public Color BorderColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 200, 200, 200).Convert();
    //public int BorderThickness { get; set; } = 1;
    //public double Opacity { get; set; } = 0.8;

    public SnapAreaTheme Theme { get; set; } = new SnapAreaTheme();
}