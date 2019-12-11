namespace SnapIt.ViewModels.DesignTime
{
    public class KeyboardSettingsDesignView
    {
        public bool EnableKeyboard { get; set; } = true;

        public string MoveUpShortcut { get; set; } = "Ctrl + Alt + Up";
        public string MoveDownShortcut { get; set; } = "Ctrl + Alt + Down";
        public string MoveLeftShortcut { get; set; } = "Ctrl + Alt + Left";
        public string MoveRightShortcut { get; set; } = "Ctrl + Alt + Right";
        public string CycleLayoutsShortcut { get; set; } = "Ctrl + Alt + C";
    }
}