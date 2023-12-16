using SnapIt.Common.Entities;

namespace SnapIt.Common.Events;

public delegate void SnappingCancelEvent();

public delegate void MoveWindowEvent(SnapAreaInfo snapAreaInfo, bool isLeftClick);