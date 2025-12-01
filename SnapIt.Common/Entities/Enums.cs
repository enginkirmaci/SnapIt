namespace SnapIt.Common.Entities;

public enum LicenseStatus
{
    InTrial,
    TrialEnded,
    Licensed
}

public enum MouseButton
{
    Left,
    Right,
    Middle,
    XButton1,
    XButton2
}

public enum HoldKey
{
    Control,
    Alt,
    Shift,
    Win
}

public enum HoldKeyBehaviour
{
    HoldToEnable,
    HoldToDisable
}

public enum UITheme
{
    Light,
    Dark,
    System
}

public enum MatchRule
{
    Contains,
    Exact,
    Wildcard
}

public enum InputDevice
{
    None,
    Both,
    Mouse,
    Keyboard
}

public enum LayoutStatus
{
    NotSaved = 0,
    Saved,
    Ignored
}

public enum SplitDirection
{
    Vertical,
    Horizontal
}

public enum ResizeHitType
{
    None, Body, UL, UR, LR, LL, L, R, T, B
}

public enum MoveDirection

{
    Left,
    Right,
    Up,
    Down
}

public enum DWMWINDOWATTRIBUTE : uint
{
    NCRenderingEnabled = 1,
    NCRenderingPolicy,
    TransitionsForceDisabled,
    AllowNCPaint,
    CaptionButtonBounds,
    NonClientRtlLayout,
    ForceIconicRepresentation,
    Flip3DPolicy,
    ExtendedFrameBounds,
    HasIconicBitmap,
    DisallowPeek,
    ExcludedFromPeek,
    Cloak,
    Cloaked,
    FreezeRepresentation,
    UseHostBackdropBrush,
    UseImmersiveDarkMode = 20,
    WindowCornerPreference = 33,
    BorderColor,
    CaptionColor,
    TextColor,
    VisibleFrameBorderThickness,
    SystemBackdropType
}

public enum DWM_WINDOW_CORNER_PREFERENCE
{
    DWMWCP_DEFAULT = 0,
    DWMWCP_DONOTROUND = 1,
    DWMWCP_ROUND = 2,
    DWMWCP_ROUNDSMALL = 3
}