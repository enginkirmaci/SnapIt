using SnapIt.Common.Entities;

namespace SnapIt.ViewModels.DesignTime;

public class SettingsDesignView
{
    public ObservableCollection<UITheme> ThemeList { get; set; }
    public UITheme SelectedTheme { get; set; } = UITheme.System;

    public SettingsDesignView()
    {
        ThemeList = new ObservableCollection<UITheme> {
            UITheme.Light,
            UITheme.Dark,
            UITheme.System
        };
    }
}