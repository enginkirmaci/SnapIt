﻿using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using SnapIt.Common.Entities;

namespace SnapIt.ViewModels.DesignTime;

public class WindowsDesignView
{
    public bool DisableForModal { get; set; }
    public ObservableCollection<ExcludedApplication> ExcludedApplications { get; set; }
    public ObservableCollection<string> RunningApplications { get; set; }
    public string SelectedApplication { get; set; }
    public bool IsRunningApplicationsDialogOpen { get; set; } = false;
    public bool IsExcludeApplicationDialogOpen { get; set; } = false;
    public ExcludedApplication SelectedExcludedApplication { get; set; }
    public ObservableCollection<MatchRule> MatchRules { get; set; }
    public DelegateCommand<ExcludedApplication> RemoveExcludedApplicationCommand { get; private set; }
    public DelegateCommand<ExcludedApplication> EditExcludedApplicationCommand { get; private set; }

    public WindowsDesignView()
    {
        DisableForModal = false;

        ExcludedApplications = new ObservableCollection<ExcludedApplication>
        {
            new ExcludedApplication
            {
                 Keyword = "Test Title",
                 MatchRule = MatchRule.Exact,
                 Mouse = true,
                 Keyboard=false
            },
            new ExcludedApplication
            {
                 Keyword = "Test Title 2",
                 MatchRule = MatchRule.Wildcard,
                 Mouse = true,
                 Keyboard=false
            },
            new ExcludedApplication
            {
                 Keyword = "Test Title 3",
                 MatchRule = MatchRule.Contains,
                 Mouse = true,
                 Keyboard=true
            },
            new ExcludedApplication
            {
                 Keyword = "Test Title 4",
                 MatchRule = MatchRule.Wildcard,
                 Mouse = false,
                 Keyboard=true
            }
        };

        SelectedExcludedApplication = ExcludedApplications.First();

        RunningApplications = new ObservableCollection<string>
        {
            "Application 1",
            "Application 2",
            "Application 3",
            "Application 4",
        };

        SelectedApplication = RunningApplications.First();

        MatchRules = new ObservableCollection<MatchRule> {
            MatchRule.Contains,
            MatchRule.Exact,
            MatchRule.Wildcard
        };
    }
}