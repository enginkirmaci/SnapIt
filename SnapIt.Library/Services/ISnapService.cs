using System.Collections.Generic;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface ISnapService
    {
        event GetStatus StatusChanged;

        event ScreenLayoutLoadedEvent ScreenLayoutLoaded;

        void Initialize();

        void Release();
    }

    public delegate void GetStatus(bool isRunning);

    public delegate void ScreenLayoutLoadedEvent(IList<SnapScreen> snapScreens, IList<Layout> layouts);
}