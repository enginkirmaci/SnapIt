using SnapIt.Common.Mvvm;

namespace SnapIt.Common.Entities;

public class ExcludedApplication : Bindable
{
    private InputDevice appliedFor;
    private bool enabledForMouse = true;
    private bool enabledForKeyboard = true;
    private MatchRule matchRule = MatchRule.Contains;
    private string keyword;

    public string Keyword { get => keyword; set => SetProperty(ref keyword, value); }
    public MatchRule MatchRule { get => matchRule; set => SetProperty(ref matchRule, value); }

    [JsonIgnore]
    public InputDevice AppliedFor
    {
        get
        {
            if (Mouse && Keyboard)
                appliedFor = InputDevice.Both;
            else if (Mouse)
                appliedFor = InputDevice.Mouse;
            else if (Keyboard)
                appliedFor = InputDevice.Keyboard;
            else
                appliedFor = InputDevice.None;

            return appliedFor;
        }
        set => SetProperty(ref appliedFor, value);
    }

    public bool Mouse
    {
        get => enabledForMouse;
        set
        {
            SetProperty(ref enabledForMouse, value);
            OnPropertyChanged("AppliedFor");
        }
    }

    public bool Keyboard
    {
        get => enabledForKeyboard;
        set
        {
            SetProperty(ref enabledForKeyboard, value);
            OnPropertyChanged("AppliedFor");
        }
    }
}