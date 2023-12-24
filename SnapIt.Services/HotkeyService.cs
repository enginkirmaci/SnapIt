using System.Windows.Input;
using GlobalHotKey;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class HotkeyService : IHotkeyService
{
    private readonly ISettingService settingService;

    private HotKeyManager hotKeyManager;

    public HotKey CycleLayoutsHotKey { get; set; }
    public HotKey StartStopHotKey { get; set; }
    public HotKey MoveLeftHotKey { get; set; }
    public HotKey MoveRightHotKey { get; set; }
    public HotKey MoveUpHotKey { get; set; }
    public HotKey MoveDownHotKey { get; set; }
    public bool IsInitialized { get; private set; }

    public event EventHandler<KeyPressedEventArgs> KeyPressed;

    public HotkeyService(
        ISettingService settingService)
    {
        this.settingService = settingService;

        hotKeyManager = new HotKeyManager();
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        await settingService.InitializeAsync();

        CycleLayoutsHotKey = HotKeyFromString(settingService.Settings.CycleLayoutsShortcut);
        StartStopHotKey = HotKeyFromString(settingService.Settings.StartStopShortcut);
        MoveLeftHotKey = HotKeyFromString(settingService.Settings.MoveLeftShortcut);
        MoveRightHotKey = HotKeyFromString(settingService.Settings.MoveRightShortcut);
        MoveUpHotKey = HotKeyFromString(settingService.Settings.MoveUpShortcut);
        MoveDownHotKey = HotKeyFromString(settingService.Settings.MoveDownShortcut);

        try
        {
            hotKeyManager.Unregister(StartStopHotKey);

            hotKeyManager.Register(CycleLayoutsHotKey);
            hotKeyManager.Register(StartStopHotKey);
            hotKeyManager.Register(MoveLeftHotKey);
            hotKeyManager.Register(MoveRightHotKey);
            hotKeyManager.Register(MoveUpHotKey);
            hotKeyManager.Register(MoveDownHotKey);
        }
        catch (Exception ex)
        {
            var t = ex;
        }

        hotKeyManager.KeyPressed -= KeyPressed;
        hotKeyManager.KeyPressed += KeyPressed;

        IsInitialized = true;
    }

    public void RegisterStartStopHotkey()
    {
        hotKeyManager.Register(StartStopHotKey);

        hotKeyManager.KeyPressed -= KeyPressed;
        hotKeyManager.KeyPressed += KeyPressed;
    }

    public void Dispose()
    {
        hotKeyManager.Unregister(CycleLayoutsHotKey);
        hotKeyManager.Unregister(StartStopHotKey);
        hotKeyManager.Unregister(MoveLeftHotKey);
        hotKeyManager.Unregister(MoveRightHotKey);
        hotKeyManager.Unregister(MoveUpHotKey);
        hotKeyManager.Unregister(MoveDownHotKey);

        hotKeyManager.KeyPressed -= KeyPressed;

        IsInitialized = false;
    }

    private HotKey HotKeyFromString(string hotkey)
    {
        if (hotkey is null)
            throw new ArgumentNullException(nameof(hotkey));

        var keys = hotkey.Split('+');

        if (!Enum.TryParse(keys[^1], ignoreCase: true, out Key regularKey))
            return new HotKey();

        Key key = regularKey;
        ModifierKeys modifierKeys = ModifierKeys.None;

        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (keys[i].Trim().Equals("Ctrl", StringComparison.InvariantCultureIgnoreCase) ||
                keys[i].Trim().Equals("Control", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Control;
            else if (keys[i].Trim().Equals("Alt", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Alt;
            else if (keys[i].Trim().Equals("Shift", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Shift;
            else if (keys[i].Trim().Equals("Win", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Windows;
        }

        return new HotKey(key, modifierKeys);
    }
}