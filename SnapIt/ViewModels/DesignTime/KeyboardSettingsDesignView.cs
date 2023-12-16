namespace SnapIt.ViewModels.DesignTime;

public class KeyboardSettingsDesignView
{
    public bool EnableKeyboard { get; set; } = true;

    public string MoveUpShortcut { get; set; } = "Control + Alt + Up";
    public string MoveDownShortcut { get; set; } = "Control + Alt + Down";
    public string MoveLeftShortcut { get; set; } = "Control + Alt + Left";
    public string MoveRightShortcut { get; set; } = "Control + Alt + Right";
    public string CycleLayoutsShortcut { get; set; } = "Control + Alt + C";
    public string StartStopShortcut { get; set; } = "Control + Alt + S";
}