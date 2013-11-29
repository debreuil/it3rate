using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using System.Drawing;
using DDW.Assets;
using DDW.Enums;
using DDW.Display;
using DDW.Utils;
using DDW.Managers;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DDW.Gdi;

namespace DDW.Controls
{
    public class Rulers : IDrawable
    {
        private StageView stage;

        public int RulerSize = 16;
        private SolidBrush rulerBrush = new SolidBrush(Color.FromArgb(255, 240, 240, 240));
        private SolidBrush rulerBrushHighlight;
        private SolidBrush rulerBrushLight1 = new SolidBrush(Color.FromArgb(255, 255, 255, 240));
        private SolidBrush rulerBrushLight2 = new SolidBrush(Color.FromArgb(255, 255, 240, 240));
        private SolidBrush textBrush = new SolidBrush(Color.Black);
        private SolidBrush boxTextBrush = new SolidBrush(Color.Green);
        private SolidBrush textPageBrush = new SolidBrush(Color.Blue);

        private Pen darkPen = new Pen(Color.Gray, 0);
        private Pen tickPen = new Pen(Color.Blue, 0);
        private Pen tickPenLight = new Pen(Color.FromArgb(255, 180, 180, 180), 0);
        private Pen rulerPen = new Pen(Color.LightGreen, 0);
        private Pen rulerPenCenter = new Pen(Color.Red, 0);

        Pen guidePen = new Pen(Color.Red, 1);

        private Rectangle topLeftBox;
        private Point[] topLeftOutline;

        private Rectangle[] rects = new Rectangle[(int)RulerBoxId.Total];
        private RulerBoxId boxId;
        private const int boxesPerRuler = 4;
        private Matrix localMatrix;

        private bool useProportionalRulers;

        
        public Rulers(StageView stage)
        {
            this.stage = stage;

            topLeftBox = new Rectangle(-RulerSize, -RulerSize, RulerSize, RulerSize);
            topLeftOutline = new Point[] { 
                new Point(-RulerSize + 1, -RulerSize + 1), 
                new Point(-RulerSize + 1, -2), 
                new Point(-2,  -2), 
                new Point(-2, -RulerSize + 1),
                new Point(-RulerSize + 1, -RulerSize + 1)};

            guidePen.DashStyle = DashStyle.Dash;
            guidePen.DashPattern = new float[] { 2f, 3f };
        }

        public void Draw(Graphics g) // root ruler
        {
            boxId = RulerBoxId.Origin;

            rulerBrushHighlight = rulerBrushLight1;

            PointF[] selPts;
            using (Matrix m = stage.GetDynamicMatrix())
            {
                if (!stage.IsEditingRoot)
                {
                    m.Multiply(stage.GetCurrentMatrix(), MatrixOrder.Append);
                }
                selPts = stage.Selection.GetTransformedPoints(m);
            }
            Rectangle selRect = selPts.GetBounds();

            Point rulerCenter = new Point(RulerSize, RulerSize);
            useProportionalRulers = true;
            using (Matrix rootM = stage.GetCalculatedRootMatrix())
            {
                DrawRulers(g, rulerCenter, new Rectangle(Point.Empty, stage.CanvasSize), stage.vexObject.ViewPort.SysRectangle(), selRect, rootM);
            }
        }

        public void DrawAt(Graphics g, Point center, Rectangle selRect, Matrix mx)
        {
            boxId = RulerBoxId.NestedOrigin;

            g.SmoothingMode = SmoothingMode.None;
            rulerBrushHighlight = rulerBrushLight2;

            Rectangle ruler = new Rectangle(0, 0, 500, 500);
            Rectangle itemBounds = new Rectangle(0, 0, 480, 480);

            useProportionalRulers = false;
            DrawRulers(g, center, ruler, itemBounds, selRect, mx);
        }

        private void DrawRulers(Graphics g, Point rulerCenter, Rectangle rulerLimits, Rectangle highlightLimits, Rectangle selRect, Matrix mx)
        {
            g.SmoothingMode = SmoothingMode.None;
            //g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            Point zeroCamera = Point.Round(GdiRenderer.GetTransformedPoint(Point.Empty, mx, false));

            //Rectangle maxRuler = new Rectangle(0, 0, 10000, 10000);
            Rectangle maxRuler = new Rectangle(Point.Empty, stage.CanvasSize);
            PointF[] pts = maxRuler.PointFs();
            GdiRenderer.TransformPoints(pts, mx, false);
            PointF tl = pts[0];
            PointF tr = pts[1];
            PointF bl = pts[3];
            double angleXRad = Math.Atan2(tr.Y - tl.Y, tr.X - tl.X);
            float angleX = (float)(angleXRad * 180f / Math.PI);
            double scaleX = (tr.X - tl.X) / (double)maxRuler.Width;
            double scaleY = (bl.Y - tl.Y) / (double)maxRuler.Height;

            GraphicsState gsOrg = g.Save();
            //g.TranslateTransform(-mx.OffsetX, -mx.OffsetY, MatrixOrder.Append);
            g.RotateTransform(angleX, MatrixOrder.Append);
            g.TranslateTransform(rulerCenter.X, rulerCenter.Y, MatrixOrder.Append);
            if (boxId == RulerBoxId.Origin)
            {
                if (localMatrix != null)
                {
                    localMatrix.Dispose();
                }
                localMatrix = g.Transform.Clone();
            }

            g.FillRectangle(rulerBrush, topLeftBox);
            g.DrawLines(darkPen, topLeftOutline);

            rects[(int)RulerBoxId.Origin + (int)boxId] = topLeftBox;

            DrawRulers(g,
                rulerLimits.Left,
                rulerLimits.Right,
                zeroCamera.X - rulerCenter.X, 
                (float)scaleX,
                highlightLimits.X,
                highlightLimits.Width,
                selRect);
            
            g.TranslateTransform(-rulerCenter.X, -rulerCenter.Y, MatrixOrder.Append);
            g.RotateTransform(-90, MatrixOrder.Append);
            g.TranslateTransform(rulerCenter.X, rulerCenter.Y, MatrixOrder.Append);

            boxId += boxesPerRuler;
            DrawRulers(g,
                -rulerLimits.Bottom,
                -rulerLimits.Top,
                zeroCamera.Y - rulerCenter.Y,
                (float)scaleY,
                highlightLimits.Height,
                highlightLimits.Y,
                selRect);

            //stage.CurrentEditItem.guides.DrawHGuides(g);

            g.Restore(gsOrg);
            g.SmoothingMode = SmoothingMode.HighQuality;
        }

        private void DrawRulers(Graphics g, int rulerLeft, int rulerRight, int zeroOffset, float scale, int paperLeft, int paperRight, Rectangle selRect)
        {
            Point left = new Point(rulerLeft, 0);
            Point right = new Point(rulerRight, 0);

            // ruler background
            Rectangle rulerBounds = new Rectangle(rulerLeft, -RulerSize, rulerRight - rulerLeft, RulerSize);
            g.FillRectangle(rulerBrush, rulerBounds);
            g.DrawLine(Pens.White, left, right);

            rects[(int)RulerBoxId.Background + (int)boxId] = rulerBounds;

            // highlight
            if (paperRight != paperLeft)
            {
                int paperWidth = paperRight - paperLeft;
                int pixelDir = (paperRight > paperLeft) ? 1 : -1;

                float highlightStart = (paperLeft * pixelDir * scale) + (zeroOffset * pixelDir);
                float highlightWidth = (Math.Abs(paperWidth * scale));
                float highlightEnd = highlightStart + highlightWidth;

                // highlight of object selection
                Rectangle highlight = new Rectangle((int)highlightStart, -RulerSize + 4, (int)highlightWidth, RulerSize - 4);
                highlight.Intersect(rulerBounds);
                g.FillRectangle(rulerBrushHighlight, highlight);

                if (useProportionalRulers)
                {
                    DrawProportionalTicks(g, rulerBounds, (int)highlightStart, (int)highlightWidth, paperLeft, paperWidth, pixelDir < 0);
                }
                else
                {
                    DrawRegularTicks(g, rulerBounds, (int)highlightStart, (int)highlightWidth, paperLeft, paperWidth, pixelDir < 0);
                }

                // selection indicators
                if (selRect.Width > 1 && selRect.Height > 1)
                {
                    int selStart;
                    int selWidth;
                    int selEnd;
                    int selL;

                    if (pixelDir > 0)
                    {
                        selStart = selRect.Left; 
                        selWidth = selRect.Width;
                        selEnd = selRect.Right;
                        selL = (int)((paperLeft + selStart) * scale) + zeroOffset;
                    }        
                    else
                    {
                        selStart = selRect.Bottom;
                        selWidth = selRect.Height;
                        selEnd = selRect.Top;
                        selL = (int)(highlightEnd - (selStart * scale));
                        //selL = (int)((paperLeft - selStart) * scale + highlightStart);
                    }
                    int selR = (int)(selL + selWidth * scale);
                    int selC = (int)((selR - selL) / 2f + selL);

                    float leftOffset = pixelDir > 0 ? 0 : 0;


                    float dist = (selR - selL) * scale;

                    if (dist > 25) // don't draw center (width) if it is too cramped
                    {
                        string centerText = pixelDir > 0 ? "":"";//"w" : "h";
                        rects[(int)RulerBoxId.Center + (int)boxId] = DrawBoxedNumber(g, centerText + selWidth.ToString("0"), selC, true);
                    }

                    if (dist > 10) // don't draw right if it is too cramped
                    {
                        rects[(int)RulerBoxId.Right + (int)boxId] = DrawBoxedNumber(g, selEnd.ToString("0"), selR, false);
                    }

                    rects[(int)RulerBoxId.Left + (int)boxId] = DrawBoxedNumber(g, selStart.ToString("0"), selL, false);
                }
            }
        }
        int[] divSteps = new int[] { 1, 2, 5, 10, 20, 50, 100, 500, 1000 };
        private void DrawRegularTicks(Graphics g, Rectangle rulerBounds, int highlightStart, int highlightWidth, int pixelsLeft, int pixelsWidth, bool drawOnLeft)
        {
            int textTL = -RulerSize + 1;
            int highlightEnd = highlightStart + highlightWidth;
            int pixelsRight = pixelsLeft + pixelsWidth;
            float scale = highlightWidth / (float)pixelsWidth;
            
            float idealDivSize = 50f;
            int divSize = divSteps[divSteps.Length - 1];
            float divWidth = Math.Abs(divSize * scale);
            for (int i = 1; i < divSteps.Length; i++)
            {
                divSize = divSteps[i];
                divWidth = Math.Abs(divSize * scale);
                if (divWidth > idealDivSize)
                {
                    if (i > 0 && divWidth - idealDivSize > idealDivSize - Math.Abs(divSteps[i - 1] * scale))
                    {
                        divSize = divSteps[i - 1];
                        divWidth = Math.Abs(divSize * scale);
                    }
                    break;
                }
            }
            divSize *= Math.Sign(pixelsWidth);
            float tickSize = divWidth / 10;

            int ht = 10;
            int px = pixelsLeft;
            for (float x = highlightStart; x <= highlightEnd; x += divWidth)
            {
                g.DrawLine(tickPenLight, new Point((int)x, -ht), new Point((int)x, 0));
                DrawStringAt(g, px.ToString(), new Point((int)x, textTL), rulerBounds, drawOnLeft);
                px += divSize;
                int count = 1;
                for (float t = tickSize; t < tickSize * 9.5; t += tickSize)
                {
                    if (t + x > highlightEnd)
                    {
                        break;
                    }
                    int tickHeight = (count == 5) ? 6 : 2;
                    g.DrawLine(tickPenLight, new Point((int)(x + t), -tickHeight), new Point((int)(x + t), 0));
                    count++;
                }
            }
        }
        private void DrawProportionalTicks(Graphics g, Rectangle rulerBounds, int highlightStart, int highlightWidth, int pixelsLeft, int pixelsWidth, bool drawOnLeft)
        {
            int textTL = -RulerSize + 1;
            int highlightEnd = highlightStart + highlightWidth;
            int pixelRight = pixelsLeft + pixelsWidth;

            float clampStart = (highlightStart > rulerBounds.Left) ? highlightStart : rulerBounds.Left;
            float clampEnd = (highlightEnd < rulerBounds.Right) ? highlightEnd : rulerBounds.Right;

            float midPx = (pixelsWidth / 2f) + pixelsLeft;
            Point startLoc = new Point(highlightStart, textTL);
            Point midLoc = new Point((int)(highlightWidth / 2f) + highlightStart, textTL);
            Point endLoc = new Point(highlightEnd, textTL);

            DrawStringAt(g, pixelsLeft.ToString(), startLoc, rulerBounds, drawOnLeft);
            DrawStringAt(g, midPx.ToString("0.##"), midLoc, rulerBounds, drawOnLeft);
            DrawStringAt(g, pixelRight.ToString(), endLoc, rulerBounds, drawOnLeft);

            float divs = 1;

            int w = highlightWidth;
            while (w > 20)
            {
                w = w >> 1;
                divs *= 2;
            }
            float space = highlightWidth / divs;
            float loc = highlightStart;
            int ht = 0;
            int count = 0;
            float end = highlightEnd + space / 2f;
            while (loc < end)
            {
                ht = (count & 1) == 1 ? 1 : (count & 2) == 2 ? 2 : (count & 4) == 4 ? 4 : (count & 8) == 8 ? 8 : 16;
                if (loc >= clampStart && loc <= clampEnd)
                {
                    g.DrawLine(tickPen, new Point((int)loc, -ht), new Point((int)loc, 0));
                }
                loc += space;
                count++;
            }
        }
        private void DrawStringAt(Graphics g, string s, Point location, Rectangle clip, bool drawOnLeft)
        {
            float stringWidth = g.MeasureString(s, FontManager.RulerFont).Width;
            int finalX = drawOnLeft ? (int)(location.X - stringWidth - 2) : location.X + 2;

            Point p = new Point(finalX, location.Y);

            if (clip.Contains(location))
            {
                g.DrawString(s, FontManager.RulerFont, textBrush, p);
            }
        }


        private Rectangle DrawBoxedNumber(Graphics g, string number, int loc, bool isCenter)
        {
            Size ssz = Size.Ceiling(g.MeasureString(number.ToString(), FontManager.RulerFont));
            int halfWidth = (int)Math.Ceiling(ssz.Width / 2f) + 2;
            Rectangle r = new Rectangle(loc - halfWidth, -RulerSize + 2, halfWidth * 2, RulerSize - 2);

            //g.DrawLine(rulerPen, new Point(loc, -RulerSize), new Point(loc, 0));

            if (isCenter)
            {
                g.FillRectangle(Brushes.Cornsilk, r);
                Rectangle r2 = new Rectangle(r.X - 2, r.Y + 2, r.Width + 4, r.Height - 4);
                int rLeft = r2.Left;
                int rRight = r2.Right;

                g.DrawLine(rulerPenCenter, new Point(loc, 0), new Point(loc, -2));
                g.DrawLine(rulerPenCenter, new Point(loc, -RulerSize), new Point(loc, -RulerSize + 2));

                g.DrawLines(rulerPenCenter, new Point[] { 
                    new Point(rLeft + 4, -RulerSize + 4), 
                    new Point(rLeft, (int)(-RulerSize / 2)), 
                    new Point(rLeft + 4, -4),
                });
                g.DrawLines(rulerPenCenter, new Point[] { 
                    new Point(rRight - 4, -RulerSize + 4), 
                    new Point(rRight, (int)(-RulerSize / 2)), 
                    new Point(rRight - 4, -4),
                   // new Point(rLeft + 4, 4) 
                });
                g.DrawString(number, FontManager.RulerFont, boxTextBrush, new Point(loc - halfWidth + 4, -RulerSize + 1));
            }
            else
            {
                g.FillRectangle(Brushes.Honeydew, r);
                Rectangle r2 = new Rectangle(r.X, r.Y + 2, r.Width, r.Height - 4);
                g.DrawLine(rulerPen, new Point(loc, 0), new Point(loc, -2));
                g.DrawLine(rulerPen, new Point(loc, -RulerSize), new Point(loc, -RulerSize + 2));

                g.DrawRectangle(rulerPen, r2);
                g.DrawString(number, FontManager.RulerFont, boxTextBrush, new Point(loc - halfWidth + 4, -RulerSize + 1));
            }

            return r;
        }
        private Rectangle RotateBack(Rectangle r)
        {
            return new Rectangle(r.Y, -r.X - r.Width, r.Height, r.Width);
        }

        private SolidBrush[] maskColors = new SolidBrush[]
        {
            new SolidBrush(ColorMask.RulerCorner.GetColor()),

            new SolidBrush(ColorMask.RulerTop.GetColor()),
            new SolidBrush(ColorMask.RulerSelLeft.GetColor()),  
            new SolidBrush(ColorMask.RulerSelWidth.GetColor()), 
            new SolidBrush(ColorMask.RulerSelRight.GetColor()), 

            new SolidBrush(ColorMask.RulerSide.GetColor()),
            new SolidBrush(ColorMask.RulerSelBottom.GetColor()),
            new SolidBrush(ColorMask.RulerSelHeight.GetColor()),
            new SolidBrush(ColorMask.RulerSelTop.GetColor())   
        };

        public void DrawMask(Graphics g)
        {
            Point rulerCenter = Point.Empty;
            if (localMatrix != null)
            {
                g.Transform = localMatrix;
            }
            else
            {
                g.TranslateTransform(RulerSize, RulerSize);
            }
            rulerCenter = new Point((int)g.Transform.OffsetX, (int)g.Transform.OffsetY);

            int startIndex = stage.IsEditingRoot ? (int)RulerBoxId.Origin : (int)RulerBoxId.NestedOrigin;
            for (int i = startIndex; i < startIndex + (int)RulerBoxId.NestedOrigin; i++)
            {
                if (i == (int)RulerBoxId.VBackground + startIndex) // rotate to y axis
                {
                    g.TranslateTransform(-rulerCenter.X, -rulerCenter.Y, MatrixOrder.Append);
                    g.RotateTransform(-90, MatrixOrder.Append);
                    g.TranslateTransform(rulerCenter.X, rulerCenter.Y, MatrixOrder.Append);
                }
                DrawMaskBox(g, maskColors[i - startIndex], rects[i]);
            }

            rects = new Rectangle[(int)RulerBoxId.Total];
            if (localMatrix != null)
            {
                localMatrix.Dispose();
                localMatrix = null;
            }
        }

        private void DrawMaskBox(Graphics g, SolidBrush b, Rectangle r)
        {
            if (r != Rectangle.Empty)
            {
                g.FillRectangle(b, r);
            }
        }

        private enum RulerBoxId : int
        {
            Origin = 0,
            Background = 1,
            Left = 2,
            Center = 3,
            Right = 4,
            VBackground = 5,
            VBottom = 6,
            VCenter = 7,
            VTop = 8,

            NestedOrigin = 9,
            NestedHBackground = 10,
            NestedHLeft = 11,
            NestedHCenter = 12,
            NestedHRight = 13,
            NestedVBackground = 14,
            NestedVBottom = 15,
            NestedVCenter = 16,
            NestedVTop = 17,

            Total = 18,
        }
    }

}
