using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;

namespace SnapIt.Application.Contracts;

public interface ISnapManager : IInitialize
{
    bool IsRunning { get; set; }

    event GetStatus StatusChanged;

    event ScreenLayoutLoadedEvent ScreenLayoutLoaded;

    event LayoutChangedEvent LayoutChanged;

    event ScreenChangedEvent ScreenChanged;

    void SetIsTrialEnded(bool isEnded);

    void ScreenChangedEvent();

    //Task StartApplications(SnapScreen snapScreen, ApplicationGroup applicationGroup);

    void StartStop();
}

public delegate void GetStatus(bool isRunning);

public delegate void LayoutChangedEvent(SnapScreen snapScreen, Layout layout);

public delegate void ScreenLayoutLoadedEvent(IList<SnapScreen> snapScreens, IList<Layout> layouts);

public delegate void ScreenChangedEvent(IList<SnapScreen> snapScreens);