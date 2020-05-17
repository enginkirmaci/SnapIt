using System;

namespace SliceRectangleSample
{
    [Flags]
    public enum SegmentPointConnections : byte
    {
        None = 0,

        Top = 1,

        Bottom = 2,

        Left = 4,

        Right = 8
    }
}