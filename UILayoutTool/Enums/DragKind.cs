
using DDW.Vex.Bonds;

namespace DDW.Enums
{
    public enum DragKind
    {
        None,
        Object,
        RectangleSelect,
        Scale,
        Rotate,
        CenterPoint,
        Pan,

        AspectConstrain,

        GuideHorizontal,
        GuideVertical,
        GuideRectangle,
    }
    
    public static class DragKindExtensions
    {
        public static bool IsGuide(this DragKind dragKind)
        {
            return
                dragKind == DragKind.GuideHorizontal ||
                dragKind == DragKind.GuideVertical ||
                dragKind == DragKind.GuideRectangle;
        }

        public static DragKind GetDragKind(this Guide guide)
        {
            DragKind result;

            switch(guide.GuideType)
            {
                case GuideType.Horizontal:
                    result = DragKind.GuideHorizontal;
                    break;
                case GuideType.Vertical:
                    result = DragKind.GuideVertical;
                    break;
                default:
                case GuideType.Rectangle:
                    result = DragKind.GuideRectangle;
                    break;
            }
            return result;
        }
    }
}