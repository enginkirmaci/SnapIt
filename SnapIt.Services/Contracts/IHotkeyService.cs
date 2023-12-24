using GlobalHotKey;
using SnapIt.Common.Contracts;

namespace SnapIt.Services.Contracts;

public interface IHotkeyService : IInitialize
{
    HotKey CycleLayoutsHotKey { get; set; }
    HotKey StartStopHotKey { get; set; }
    HotKey MoveLeftHotKey { get; set; }
    HotKey MoveRightHotKey { get; set; }
    HotKey MoveUpHotKey { get; set; }
    HotKey MoveDownHotKey { get; set; }

    event EventHandler<KeyPressedEventArgs> KeyPressed;

    void RegisterStartStopHotkey();
}