using System.Text.RegularExpressions;
using SnapIt.Common.Entities;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class WindowsService : IWindowsService
{
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private List<ExcludedApplication> matchRulesForMouse;
    private List<ExcludedApplication> matchRulesForKeyboard;

    public bool IsInitialized { get; private set; }

    public WindowsService(
       ISettingService settingService,
       IWinApiService winApiService)
    {
        this.settingService = settingService;
        this.winApiService = winApiService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        await settingService.InitializeAsync();

        if (settingService.ExcludedApplicationSettings?.Applications != null)
        {
            matchRulesForMouse = settingService.ExcludedApplicationSettings.Applications.Where(i => i.Mouse).ToList();
            matchRulesForKeyboard = settingService.ExcludedApplicationSettings.Applications.Where(i => i.Keyboard).ToList();
        }

        IsInitialized = true;
    }

    public bool DisableIfFullScreen()
    {
        var activeWindow = winApiService.GetActiveWindow();
        if (activeWindow != ActiveWindow.Empty && !string.IsNullOrWhiteSpace(activeWindow.Title) && !activeWindow.Title.Equals("Program Manager") && settingService.Settings.DisableForFullscreen && winApiService.IsFullscreen(activeWindow))
        {
            return true;
        }
        return false;
    }

    public bool IsExcludedApplication(string Title, bool isKeyboard)
    {
        if (settingService.ExcludedApplicationSettings?.Applications != null)
        {
            var matchRules = isKeyboard ? matchRulesForKeyboard : matchRulesForMouse;
            if (matchRules != null)
            {
                var isMatched = false;
                foreach (var rule in matchRules)
                {
                    if (string.IsNullOrWhiteSpace(rule.Keyword))
                    {
                        continue;
                    }

                    switch (rule.MatchRule)
                    {
                        case MatchRule.Contains:
                            isMatched = Title.Contains(rule.Keyword, StringComparison.OrdinalIgnoreCase);
                            break;

                        case MatchRule.Exact:
                            isMatched = Title == rule.Keyword;
                            break;

                        case MatchRule.Wildcard:
                            isMatched = WildcardMatch(rule.Keyword, Title, false);
                            break;
                    }

                    if (isMatched)
                    {
                        break;
                    }
                }

                return isMatched;
            }
        }

        return false;
    }

    public void Dispose()
    {
        IsInitialized = false;
    }

    private bool WildcardMatch(string pattern, string input, bool caseSensitive = false)
    {
        pattern = pattern.Replace(".", @"\.");
        pattern = pattern.Replace("?", ".");
        pattern = pattern.Replace("*", ".*?");
        pattern = pattern.Replace(@"\", @"\\");
        pattern = pattern.Replace(" ", @"\s");
        return new Regex(pattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase).IsMatch(input);
    }
}