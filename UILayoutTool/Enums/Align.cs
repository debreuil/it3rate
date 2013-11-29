using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.Enums
{
    [Flags]
    public enum VerticalAlignType
    {
        None = 0,
        Left,
        Center,
        Right,
        Distribute,
        Grid,
        IsAligned = Left | Center | Right,
    }

    [Flags]
    public enum HorizontalAlignType
    {
        None = 0,
        Top,
        Center,
        Bottom,
        Distribute,
        Grid,
        IsAligned = Top | Center | Bottom,
    }
}
