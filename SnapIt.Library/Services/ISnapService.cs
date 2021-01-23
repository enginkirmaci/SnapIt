using System.Collections.Generic;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface ISnapService
    {
        event GetStatus StatusChanged;

        event ScreenLayoutLoadedEvent ScreenLayoutLoaded;

        event LayoutChangedEvent LayoutChanged;

        event ScreenChangedEvent ScreenChanged;

        void ScreenChangedEvent();

        void Initialize();

        void Release();
    }

    public delegate void GetStatus(bool isRunning);

    public delegate void LayoutChangedEvent(SnapScreen snapScreen, Layout layout);

    public delegate void ScreenLayoutLoadedEvent(IList<SnapScreen> snapScreens, IList<Layout> layouts);

    public delegate void ScreenChangedEvent(IList<SnapScreen> snapScreens);
}