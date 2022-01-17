using System.Collections.Generic;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SnapScreen.Library.Entities
{
    public class Settings
    {
        public Settings()
        {
            ScreensLayouts = new Dictionary<string, string>();
            ActiveScreens = new List<string>();
        }

        public string Version = "1.0";
        public bool EnableMouse { get; set; } = true;
        public bool DragByTitle { get; set; } = true;

        [JsonConverter(typeof(StringEnumConverter))]
        public MouseButton MouseButton { get; set; } = MouseButton.Left;

        public int MouseDragDelay { get; set; } = 200;
        public bool EnableHoldKey { get; set; } = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public HoldKey HoldKey { get; set; } = HoldKey.Control;

        [JsonConverter(typeof(StringEnumConverter))]
        public HoldKeyBehaviour HoldKeyBehaviour { get; set; } = HoldKeyBehaviour.HoldToEnable;

        public bool EnableKeyboard { get; set; } = true;
        public bool DisableForFullscreen { get; set; } = true;
        public bool DisableForModal { get; set; } = true;
        public bool ShowMainWindow { get; set; } = true;

        [JsonConverter(typeof(StringEnumConverter))]
        public UITheme AppTheme { get; set; } = UITheme.System;

        public AccentColorData AppAccentColor { get; set; } = new AccentColorData
        {
            Name = "Blue",
            Color = Color.FromArgb(255, 0, 120, 215)
        };

        public Dictionary<string, string> ScreensLayouts { get; set; }
        public List<string> ActiveScreens { get; set; }
        public string MoveUpShortcut { get; set; } = "Control + Alt + Up";
        public string MoveDownShortcut { get; set; } = "Control + Alt + Down";
        public string MoveLeftShortcut { get; set; } = "Control + Alt + Left";
        public string MoveRightShortcut { get; set; } = "Control + Alt + Right";
        public string CycleLayoutsShortcut { get; set; } = "Control + Alt + C";
        public string StartStopShortcut { get; set; } = "Control + Alt + S";
        public bool IsVersion3000MessageShown { get; set; }

        public SnapAreaTheme Theme { get; set; } = new SnapAreaTheme();
        public bool CheckForNewVersion { get; set; } = true;
    }
}