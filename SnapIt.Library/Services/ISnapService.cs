using System.Collections.Generic;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface ISnapService
    {
        bool IsRunning { get; set; }

        event GetStatus StatusChanged;

        event ScreenLayoutLoadedEvent ScreenLayoutLoaded;

        event LayoutChangedEvent LayoutChanged;

        event ScreenChangedEvent ScreenChanged;

        void SetIsTrialEnded(bool isEnded);

        void ScreenChangedEvent();

        void StartApplications(SnapScreen snapScreen, ApplicationGroup applicationGroup);

        void Initialize();

        void Release();
    }

    public delegate void GetStatus(bool isRunning);

    public delegate void LayoutChangedEvent(Entities.SnapScreen snapScreen, Layout layout);

    public delegate void ScreenLayoutLoadedEvent(IList<Entities.SnapScreen> snapScreens, IList<Layout> layouts);

    public delegate void ScreenChangedEvent(IList<Entities.SnapScreen> snapScreens);
}