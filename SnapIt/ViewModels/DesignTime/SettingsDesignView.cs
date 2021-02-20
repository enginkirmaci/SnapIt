using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using ControlzEx.Theming;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels.DesignTime
{
    public class SettingsDesignView
    {
        public IEnumerable<AccentColorData> AccentColors { get; set; }

        public AccentColorData SelectedAccentColor { get; set; }

        public ObservableCollection<UITheme> ThemeList { get; set; }
        public UITheme SelectedTheme { get; set; } = UITheme.System;

        public SettingsDesignView()
        {
            AccentColors = ThemeManager.Current.Themes
                                            .GroupBy(x => x.ColorScheme)
                                            .OrderBy(a => a.Key)
                                            .Select(a => new AccentColorData { Name = a.Key, Color = ((SolidColorBrush)a.First().ShowcaseBrush).Color })
                                            .ToList();

            SelectedAccentColor = AccentColors.ElementAt(2);

            ThemeList = new ObservableCollection<UITheme> {
                UITheme.Light,
                UITheme.Dark,
                UITheme.System
            };
        }
    }
}