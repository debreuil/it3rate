using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Display;
using DDW.Utils;
using Vex = DDW.Vex;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DDW.Assets;
using DDW.Enums;
using DDW.Vex.Bonds;

namespace DDW.Controls
{
    public class Guidelines
    {
        private DesignTimeline timeline;
        private Dictionary<uint, Guide> guides;

        public Guide draggingGuide;

        public static Pen guidePen = new Pen(Color.Blue, 0);
        public static Pen guidePenError = new Pen(Color.Red, 0);
        private static Pen guidePenLight = new Pen(Color.Gray, 0);

        private uint colorHashCounter = (uint)ColorMask.First;

        public Guidelines(DesignTimeline timeline)
        {
            this.timeline = timeline;
            this.guides = new Dictionary<uint, Guide>();

            //AddGuide(new Guide(new Vex.Rectangle(200, 0, 0, 400)));
            //AddGuide(new Guide(new Vex.Rectangle(0, 150, 550, 0)));
            //AddGuide(new Guide(new Vex.Rectangle(55, 88, 122, 99)));

            guidePen.DashStyle = DashStyle.Dash;
            guidePen.DashPattern = new float[] { 2f, 3f };
            guidePenError.DashStyle = DashStyle.Dash;
            guidePenError.DashPattern = new float[] { 2f, 3f };
            guidePenLight.DashStyle = DashStyle.Dash;
            guidePenLight.DashPattern = new float[] { 2f, 3f };
        }

        public int Count { get { return guides.Count; } }
        public Guide[] Guides { get { return guides.Values.ToArray(); } }

        public void AddGuide(Guide guide)
        {
            guide.InstanceHash = colorHashCounter--;
            guides.Add(guide.InstanceHash, guide);

            if (guide.GuideType == GuideType.Point || guide.GuideType == GuideType.Rectangle)
            {
                guides.Add(colorHashCounter--, guide);
            }

            if (guide.GuideType == GuideType.Rectangle)
            {
                guides.Add(colorHashCounter--, guide);
                guides.Add(colorHashCounter--, guide);
            }
        }
        public void RemoveGuide(Guide guide)
        {
            guides.Remove(guide.InstanceHash);

            if (guide.GuideType == GuideType.Point || guide.GuideType == GuideType.Rectangle)
            {
                guides.Remove(guide.InstanceHash - 1);
            }

            if (guide.GuideType == GuideType.Rectangle)
            {
                guides.Remove(guide.InstanceHash - 2);
                guides.Remove(guide.InstanceHash - 3);
            }
        }
        public Guide[] GetHGuides()
        {
            List<Guide> result = new List<Guide>();
            foreach (uint key in guides.Keys)
            {
                Guide guide = guides[key];
                if (guide.IsVertical)
                {
                    result.Add(guide);
                }
            }
            return result.ToArray();
        }
        public Guide[] GetVGuides()
        {
            List<Guide> result = new List<Guide>();
            foreach (uint key in guides.Keys)
            {
                Guide guide = guides[key];
                if (guide.IsVertical)
                {
                    result.Add(guide);
                }
            }
            return result.ToArray();
        }

        private uint GuideColorFromGuide(Guide guide)
        {
            return guide.InstanceHash;
        }
        public Guide GetGuideFromColor(uint color)
        {
            Guide result = null;
            if (guides.ContainsKey(color))
            {
                result = guides[color];
            }
            return result;
        }

        public void Draw(Graphics g, bool isFocused)
        {
            //g.SmoothingMode = SmoothingMode.None;
            //g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            Pen p = isFocused ? guidePen : guidePenLight;
            foreach (uint key in guides.Keys)
            {
                DrawGuide(g, p, guides[key]);
            }

            if (draggingGuide != null)
            {
                DrawGuide(g, p, draggingGuide);
            }
        }

        private void DrawGuide(Graphics g, Pen p, Guide guide)
        {
            switch (guide.GuideType)
            {
                case GuideType.Point:
                    break;
                case GuideType.Horizontal:
                case GuideType.Vertical:
                    g.DrawLine(p, guide.Bounds.Point.SysPoint(), guide.EndPoint.SysPoint());
                    break;
                case GuideType.Rectangle:
                    g.DrawRectangle(p, guide.Bounds.SysRectangle());
                    break;
            }
        }

        public void DrawMask(Graphics g)
        {
            foreach (uint key in guides.Keys)
            {
                Guide guide = guides[key];
                uint c = GuideColorFromGuide(guide);
                GuideType gt = guide.GuideType;
                switch (gt)
                {
                    case GuideType.Point:
                        break;
                    case GuideType.Horizontal:
                    case GuideType.Vertical:
                        using (Pen p = new Pen(Color.FromArgb((int)c), 5f))
                        {
                            g.DrawLine(p, guide.Bounds.Point.SysPoint(), guide.EndPoint.SysPoint());
                        }
                        break;
                    case GuideType.Rectangle:
                        PointF[] pts = guide.Bounds.SysRectangleF().Points();
                        for (int i = 0; i < pts.Length; i++)
                        {
                            using (Pen p = new Pen(Color.FromArgb((int)c - i), 5f))
                            {
                                int targPoint = (i == pts.Length - 1) ? 0 : i + 1;
                                g.DrawLine(p, pts[i], pts[targPoint]);
                            }
                            
                        }
                        break;
                }
            }
        }
    }
}
