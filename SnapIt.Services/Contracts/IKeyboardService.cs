using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Services.Contracts;

public delegate void SnappingCancelEvent();

public delegate void SnapStartStopEvent();

public delegate void MoveWindowEvent(SnapAreaInfo snapAreaInfo, bool isLeftClick);

public delegate IList<Rectangle> GetSnapAreaBoundriesEvent();

public delegate void ChangeLayoutEvent(SnapScreen snapScreen, Layout layout);

public interface IKeyboardService : IInitialize, IDisposable
{
    event SnappingCancelEvent SnappingCancelled;

    event SnapStartStopEvent SnapStartStop;

    event MoveWindowEvent MoveWindow;

    event GetSnapAreaBoundriesEvent GetSnapAreaBoundries;

    event ChangeLayoutEvent ChangeLayout;

    void SetSnappingStopped();
}