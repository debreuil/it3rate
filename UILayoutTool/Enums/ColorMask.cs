using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DDW.Enums
{
    public enum ColorMask : uint
    {
        First = 0xFFFFFF00,

        ScaleTopLeft,
        ScaleTopCenter,
        ScaleTopRight,
        ScaleRightCenter,
        ScaleBottomRight,
        ScaleBottomCenter,
        ScaleBottomLeft,
        ScaleLeftCenter,

        RotateTopLeft,
        RotateTopCenter,
        RotateTopRight,
        RotateRightCenter,
        RotateBottomCenter,
        RotateBottomRight,
        RotateBottomLeft,
        RotateLeftCenter,

        CenterPoint,

        RulerCorner,
        RulerTop,
        RulerSide,

        RulerSelLeft,
        RulerSelRight,
        RulerSelTop,
        RulerSelBottom,

        RulerSelWidth,
        RulerSelHeight,

        Last,
    }

    public static class ColorMaskExtensions
    {
        public static Color GetColor(this ColorMask colorMask)
        {
            return Color.FromArgb((int)colorMask);
        }

        public static bool IsHandle(this ColorMask colorMask)
        {
            return (colorMask > ColorMask.First) && (colorMask < ColorMask.CenterPoint);
        }
        public static int GetHandleIndex(this ColorMask colorMask)
        {
            int result = -1;

            if ((colorMask > ColorMask.First) && (colorMask < ColorMask.CenterPoint))
            {
                switch (colorMask)
                {
                    case ColorMask.ScaleTopLeft:
                    case ColorMask.RotateTopLeft:
                        result = 0;
                        break;
                    case ColorMask.ScaleTopCenter:
                    case ColorMask.RotateTopCenter:
                        result = 1;
                        break;
                    case ColorMask.ScaleTopRight:
                    case ColorMask.RotateTopRight:
                        result = 2;
                        break;
                    case ColorMask.ScaleRightCenter:
                    case ColorMask.RotateRightCenter:
                        result = 3;
                        break;
                    case ColorMask.ScaleBottomRight:
                    case ColorMask.RotateBottomRight:
                        result = 4;
                        break;
                    case ColorMask.ScaleBottomCenter:
                    case ColorMask.RotateBottomCenter:
                        result = 5;
                        break;
                    case ColorMask.ScaleBottomLeft:
                    case ColorMask.RotateBottomLeft:
                        result = 6;
                        break;
                    case ColorMask.ScaleLeftCenter:
                    case ColorMask.RotateLeftCenter:
                        result = 7;
                        break;
                }
            }
            return result;
        }

        public static int GetOppositeHandleIndex(this ColorMask colorMask)
        {
            int result = -1;

            if ((colorMask > ColorMask.First) && (colorMask < ColorMask.CenterPoint))
            {
                switch (colorMask)
                {
                    case ColorMask.ScaleTopLeft:
                    case ColorMask.RotateTopLeft:
                        result = 4;
                        break;
                    case ColorMask.ScaleTopCenter:
                    case ColorMask.RotateTopCenter:
                        result = 5;
                        break;
                    case ColorMask.ScaleTopRight:
                    case ColorMask.RotateTopRight:
                        result = 6;
                        break;
                    case ColorMask.ScaleRightCenter:
                    case ColorMask.RotateRightCenter:
                        result = 7;
                        break;
                    case ColorMask.ScaleBottomRight:
                    case ColorMask.RotateBottomRight:
                        result = 0;
                        break;
                    case ColorMask.ScaleBottomCenter:
                    case ColorMask.RotateBottomCenter:
                        result = 1;
                        break;
                    case ColorMask.ScaleBottomLeft:
                    case ColorMask.RotateBottomLeft:
                        result = 2;
                        break;
                    case ColorMask.ScaleLeftCenter:
                    case ColorMask.RotateLeftCenter:
                        result = 3;
                        break;
                }
            }

            return result;
        }

        public static bool IsObject(this ColorMask colorMask)
        {
            return (colorMask < ColorMask.First);
        }
        public static bool IsScale(this ColorMask colorMask)
        {
            return  ((colorMask >= ColorMask.ScaleTopLeft) && (colorMask <= ColorMask.ScaleLeftCenter)) ||
                    ((colorMask >= ColorMask.RulerSelLeft) && (colorMask <= ColorMask.RulerSelBottom));
        }
        public static bool IsRotate(this ColorMask colorMask)
        {
            return (colorMask >= ColorMask.RotateTopLeft) && (colorMask <= ColorMask.RotateLeftCenter);
        }
        public static bool IsLeft(this ColorMask colorMask)
        {
            return  (colorMask == ColorMask.ScaleTopLeft) || (colorMask == ColorMask.ScaleBottomLeft) ||
                    (colorMask == ColorMask.RotateTopLeft) || (colorMask == ColorMask.RotateBottomLeft) ||
                    (colorMask == ColorMask.ScaleLeftCenter) ||
                    (colorMask == ColorMask.RotateLeftCenter) ||
                    (colorMask == ColorMask.RulerSelLeft);
        }
        public static bool IsTop(this ColorMask colorMask)
        {
            return (colorMask == ColorMask.ScaleTopLeft) || (colorMask == ColorMask.ScaleTopRight) ||
                    (colorMask == ColorMask.RotateTopLeft) || (colorMask == ColorMask.RotateTopRight) ||
                    (colorMask == ColorMask.ScaleTopCenter) ||
                    (colorMask == ColorMask.RotateTopCenter) ||
                    (colorMask == ColorMask.RulerSelTop);
        }

        public static bool IsScaleConstrained(this ColorMask colorMask)
        {
            return (colorMask == ColorMask.ScaleTopCenter) ||
                (colorMask == ColorMask.ScaleRightCenter)   ||
                (colorMask == ColorMask.ScaleBottomCenter)  ||
                (colorMask == ColorMask.ScaleLeftCenter)    ||
                ((colorMask >= ColorMask.RulerSelLeft) && (colorMask <= ColorMask.RulerSelBottom));
        }
        public static bool IsRuler(this ColorMask colorMask)
        {
            return (colorMask >= ColorMask.RulerCorner) && (colorMask <= ColorMask.RulerSelHeight);
        }

        public static Vex.Point GetScalingOrigin(this ColorMask colorMask, Vex.Rectangle r)
        {
            Vex.Point result = r.Point;
            switch (colorMask)
            {
                case ColorMask.ScaleTopLeft:
                case ColorMask.ScaleLeftCenter:
                case ColorMask.ScaleTopCenter:
                case ColorMask.RulerSelLeft:
                case ColorMask.RulerSelTop:
                    result = new Vex.Point(r.Right, r.Bottom);
                    break;

                case ColorMask.ScaleTopRight:
                    result = new Vex.Point(r.Left, r.Bottom);
                    break;

                case ColorMask.ScaleBottomRight:
                case ColorMask.ScaleBottomCenter:
                case ColorMask.ScaleRightCenter:
                case ColorMask.RulerSelRight:
                case ColorMask.RulerSelBottom:
                    result = r.Point;
                    break;

                case ColorMask.ScaleBottomLeft:
                    result = new Vex.Point(r.Right, r.Top);
                    break;

            }
            return result;
        }
        public static PointF GetScalingOrigin(this ColorMask colorMask, PointF[] pts)
        {
            PointF result = PointF.Empty;
            if (pts.Length == 4)
            {
                switch (colorMask)
                {
                    case ColorMask.ScaleTopLeft:
                    case ColorMask.ScaleLeftCenter:
                    case ColorMask.ScaleTopCenter:
                    case ColorMask.RulerSelLeft:
                    case ColorMask.RulerSelTop:
                        result = pts[2];
                        break;

                    case ColorMask.ScaleTopRight:
                        result = pts[3];
                        break;

                    case ColorMask.ScaleBottomRight:
                    case ColorMask.ScaleBottomCenter:
                    case ColorMask.ScaleRightCenter:
                    case ColorMask.RulerSelRight:
                    case ColorMask.RulerSelBottom:
                        result = pts[0];
                        break;

                    case ColorMask.ScaleBottomLeft:
                        result = pts[1];
                        break;
                }
            }
            return result;
        }
        public static Vex.Point GetHandleOffset(this ColorMask colorMask, Vex.Rectangle r, Vex.Point mouseDownLocation)
        {
            Vex.Point result = Vex.Point.Zero;
            switch (colorMask)
            {
                case ColorMask.ScaleTopLeft:
                case ColorMask.RotateTopLeft:
                    result = mouseDownLocation.Translate(new Vex.Point(-r.Left, -r.Top));
                    break;

                case ColorMask.ScaleTopRight:
                case ColorMask.RotateTopRight:
                    result = mouseDownLocation.Translate(new Vex.Point(-r.Right, -r.Top));
                    break;

                case ColorMask.ScaleBottomRight:
                case ColorMask.RotateBottomRight:
                    result = mouseDownLocation.Translate(new Vex.Point(-r.Right, -r.Bottom));
                    break;

                case ColorMask.ScaleBottomLeft:
                case ColorMask.RotateBottomLeft:
                    result = mouseDownLocation.Translate(new Vex.Point(-r.Left, -r.Bottom));
                    break;

                case ColorMask.RulerSelLeft:
                    result = mouseDownLocation.Translate(new Vex.Point(-r.Left, 0));
                    break;
                case ColorMask.RulerSelTop:
                    result = mouseDownLocation.Translate(new Vex.Point(0, -r.Top));
                    break;
                case ColorMask.RulerSelRight:
                    result = mouseDownLocation.Translate(new Vex.Point(-r.Right, 0));
                    break;
                case ColorMask.RulerSelBottom:
                    result = mouseDownLocation.Translate(new Vex.Point(0, -r.Bottom));
                    break;
            }
            return result;
        }

    }
}
