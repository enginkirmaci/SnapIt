using SnapIt.Common.Contracts;
using SnapIt.Common.Events;

namespace SnapIt.Services.Contracts;

public interface IMouseService : IInitialize
{
    void Interrupt();

    event HideWindowsEvent HideWindows;

    event MoveWindowEvent MoveWindow;

    event SnappingCancelEvent SnappingCancelled;

    event ShowWindowsIfNecessaryEvent ShowWindowsIfNecessary;

    event SelectElementWithPointEvent SelectElementWithPoint;
}