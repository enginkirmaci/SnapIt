using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Common.Events;

public delegate void HideWindowsEvent();

public delegate bool ShowWindowsIfNecessaryEvent();

public delegate SnapAreaInfo SelectElementWithPointEvent(int x, int y);

public delegate IList<Rectangle> GetSnapAreaBoundriesEvent();