/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;

using Vex = DDW.Vex;
using DDW.Assets;
using DDW.Managers;
using DDW.Display;
using DDW.Utils;
using DDW.Views;

namespace DDW.Gdi
{
    public class GdiRenderer
    {
        private Graphics g;
        private StageView stage;

        private SolidBrush fillOverride;
        private Pen penOverride;
        private Pen outlinePen = Pens.Black;
        
        public GdiRenderer(StageView stage)
        {
            this.stage = stage;
            outlinePen = new Pen(Color.DarkBlue, 0);
            outlinePen.StartCap = LineCap.Round;
            outlinePen.EndCap = LineCap.Round;
            outlinePen.LineJoin = LineJoin.Round;
        }

        #region Render Methods
        public Bitmap RenderFirstFrame(Vex.IDefinition def)
        {
            using(Matrix m = new Matrix())
            {
                return RenderDefinitionFrames(def, 0, 1, m)[0];
            }
        }
        public Bitmap RenderFirstFrame(Vex.IDefinition def, Vex.Matrix m)
        {
            using (Matrix sm = new Matrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, 0, 0))
            {
                return RenderDefinitionFrames(def, 0, 1, sm)[0];
            }
        }
        public Bitmap RenderFirstFrame(Vex.IDefinition def, Matrix m)
        {
            using (Matrix sm = new Matrix(m.Elements[0], m.Elements[1], m.Elements[2], m.Elements[3], 0, 0))
            {
                return RenderDefinitionFrames(def, 0, 1, sm)[0];
            }
        }
        public List<Bitmap> RenderDefinitionFrames(Vex.IDefinition def, uint startFrame, uint endFrame, Matrix m)
        {
            List<Bitmap> bmpFrames = new List<Bitmap>();

            if (def is Vex.Timeline)
            {
                Vex.Timeline tl = (Vex.Timeline)def;
                Vex.Timeline namedSymbol = GetNamedSymbol(tl);
                if (namedSymbol != null)// && HasSymbols(namedSymbol))
                {
                    for (uint i = startFrame; i < endFrame; i++)
                    {
                        m.Translate(-namedSymbol.StrokeBounds.Point.X, -namedSymbol.StrokeBounds.Point.Y);
                        Bitmap myBitmap = CreateRenderBitmap(namedSymbol.StrokeBounds, m);
                        RenderFrame(namedSymbol, i);
                        bmpFrames.Add(myBitmap);
                    }
                }
            }
            else if (def is Vex.Symbol)
            {
                Vex.Symbol sy = (Vex.Symbol)def;

                m.Translate(-sy.StrokeBounds.Point.X, -sy.StrokeBounds.Point.Y);
                Bitmap myBitmap = CreateRenderBitmap(sy.StrokeBounds, m);

                RenderFrame(sy, 0);
                bmpFrames.Add(myBitmap);
            }
            else if (def is Vex.Text)
            {
                Vex.Text tx = (Vex.Text)def;

                if (tx.TextRuns.Count > 0 && !tx.TextRuns[0].isEditable)
                {
                    Bitmap myBitmap = CreateRenderBitmap(tx.StrokeBounds, m);
                    RenderFrame(tx, 0);
                    bmpFrames.Add(myBitmap);
                }
            }
            else if (def is Vex.Image)
            {
                Bitmap myBitmap;
                m.Translate(-def.StrokeBounds.Point.X, -def.StrokeBounds.Point.Y);
                if (m.IsScaledOrSheared())
                {
                    myBitmap = CreateRenderBitmap(def.StrokeBounds, m);
                    Bitmap sourceBmp = stage.BitmapCache.GetBitmap(def);
                    using (Graphics gr = Graphics.FromImage(myBitmap))
                    {
                        gr.DrawImage(sourceBmp, 0, 0, myBitmap.Width, myBitmap.Height);
                    }
                }
                else                
                {
                    myBitmap = stage.BitmapCache.GetBitmap(def);
                }                

                bmpFrames.Add(myBitmap);
            }

            if (g != null)
            {
                g.Dispose();
            }

            return bmpFrames;
        }
        public void ClearBitmap(Bitmap image)
        {
            this.g = GetGraphicsFromBitmap(image);
            this.g.Clear(Color.White);
            this.g.Dispose();
            this.g = null;
        }
        public void RenderInto(Vex.IDefinition def, uint frame, Bitmap image)
        {
            SizeF targSize = image.Size;
            SizeF sourceSize = def.StrokeBounds.Size.SysSize();
            if (sourceSize.Width != 0 && sourceSize.Height != 0)
            {
                float wRatio = targSize.Width / sourceSize.Width;
                float hRatio = targSize.Height / sourceSize.Height;

                float finalRatio;
                int offsetX = 0;
                int offsetY = 0;
                if (wRatio > hRatio)
                {
                    finalRatio = hRatio;
                    offsetX = (int)((targSize.Width - sourceSize.Width * hRatio) / 2.0);
                }
                else
                {
                    finalRatio = wRatio;
                    offsetY = (int)((targSize.Height - sourceSize.Height * wRatio) / 2.0);
                }

                Matrix translateMatrix = new Matrix(
                    finalRatio, 0, 0, finalRatio,
                    -def.StrokeBounds.Point.X * finalRatio + offsetX,
                    -def.StrokeBounds.Point.Y * finalRatio + offsetY);
                this.g = GetGraphicsFromBitmap(image);

                this.g.Clear(Color.Transparent);
                this.g.Transform = translateMatrix;

                RenderFrame(def, 0);

                this.g.Transform.Dispose();
                this.g.Dispose();
                this.g = null;
            }
        }

        public void RenderOutlineInto(Vex.IDefinition def, uint frame, Graphics graphics)
        {
            penOverride = outlinePen;
            fillOverride = (SolidBrush)Brushes.Transparent;

            RenderInto(def, frame, graphics);

            fillOverride = null;
            penOverride = null;
        }
        public void RenderMaskInto(Vex.IDefinition def, uint frame, Graphics graphics, Color c)
        {
            penOverride = new Pen(c, 1);
            fillOverride = new SolidBrush(c);

            RenderInto(def, frame, graphics);

            fillOverride.Dispose();
            fillOverride = null;
            penOverride.Dispose();
            penOverride = null;
        }
        public void RenderInto(Vex.IDefinition def, uint frame, Graphics graphics)
        {
            this.g = graphics;

            if (def is Vex.Timeline)
            {
                Vex.Timeline tl = (Vex.Timeline)def;
                Vex.Timeline namedSymbol = GetNamedSymbol(tl);
                if (namedSymbol != null)// && HasSymbols(namedSymbol))
                {
                    RenderFrame(namedSymbol, frame);
                }
            }
            else if (def is Vex.Symbol)
            {
                Vex.Symbol sy = (Vex.Symbol)def;
                RenderFrame(sy, 0);
            }
            else if (def is Vex.Text)
            {
                Vex.Text tx = (Vex.Text)def;
                if (tx.TextRuns.Count > 0 && !tx.TextRuns[0].isEditable)
                {
                    RenderFrame(tx, 0);
                }
            }
            else if (def is Vex.Image)
            {
                Bitmap sourceBmp = stage.BitmapCache.GetBitmap(def);
                DrawImage(sourceBmp);
            }
        }
        private void DrawImage(Bitmap sourceBmp)
        {
            if (fillOverride != null && fillOverride != Brushes.Transparent)
            {
                Bitmap maskedBmp = FastBitmap.GetMask(sourceBmp, fillOverride.Color);
                g.DrawImage(maskedBmp, 0, 0, maskedBmp.Width, maskedBmp.Height);
                maskedBmp.Dispose();
            }
            else if (penOverride != null)
            {
                Bitmap ghostBmp = FastBitmap.GetGhost(sourceBmp);
                g.DrawImage(ghostBmp, 0, 0, ghostBmp.Width, ghostBmp.Height);
                ghostBmp.Dispose();
            }
            else
            {
                g.DrawImage(sourceBmp, 0, 0, sourceBmp.Width, sourceBmp.Height);
            }
        }
        // assumes graphics, matrix are setup
        private void RenderFrame(Vex.IDefinition def, uint frame)
        {
            if (def is Vex.Timeline)
            {
                DrawFilteredTimeline((Vex.Timeline)def, frame);
            }
            else if (def is Vex.Symbol)
            {
                DrawFilteredSymbol((Vex.Symbol)def);
            }
            else if (def is Vex.Text)
            {
                Vex.Text tx = (Vex.Text)def;

                if (tx.TextRuns.Count > 0 && !tx.TextRuns[0].isEditable)
                {
                    DrawText(tx, new DDW.Vex.Matrix(1, 0, 0, 1, -tx.StrokeBounds.Point.X, -tx.StrokeBounds.Point.Y));
                }
            }
            else if (def is Vex.Image)
            {
                DrawImage(stage.BitmapCache.GetBitmap(def));
            }
        }
        #endregion

        private void DrawTimeline(Vex.Timeline tl)
        {
            for (int i = 0; i < tl.InstanceCount; i++)
            {
                Vex.IDefinition idef = stage.Library[tl.InstanceAt(i).DefinitionId].Definition;
                if (idef is Vex.Symbol)
                {
                    DrawSymbol((Vex.Symbol)idef);
                }
                else if (idef is Vex.Timeline)
                {
                    DrawTimeline((Vex.Timeline)idef);
                }
            }
        }
        private void DrawFilteredSymbol(Vex.Symbol sy)
        {
            foreach (Vex.Shape sh in sy.Shapes)
            {
                if (!sh.IsV2DShape())
                {
                    DrawShape(sh);
                }
            }
        }
        // filtered meaning remove annotations like box2d bounding boxes
        private void DrawFilteredTimeline(Vex.Timeline tl, uint frame)
        {
            for (int i = 0; i < tl.InstanceCount; i++)
            {
                Vex.Instance inst = (Vex.Instance)tl.InstanceAt(i);
                if (inst.GetTransformAtTime(frame) != null)
                {
                    Vex.IDefinition idef = stage.Library[inst.DefinitionId].Definition;
                    if (idef is Vex.Symbol || idef is Vex.Timeline)
                    {
                        GraphicsState gs = g.Save();
                        using (Matrix nm = inst.GetTransformAtTime(0).Matrix.GetDrawing2DMatrix())
                        {
                            nm.Multiply(g.Transform, MatrixOrder.Append);
                            g.Transform = nm;
                            if (idef is Vex.Symbol)
                            {
                                DrawFilteredSymbol((Vex.Symbol)idef);
                            }
                            else
                            {
                                DrawFilteredTimeline((Vex.Timeline)idef, frame);
                            }
                        }
                        g.Restore(gs);
                    }
                    else if (idef is Vex.Text)
                    {
                        DrawText((Vex.Text)idef, inst.Transformations[0].Matrix);
                    }
                    else if (idef is Vex.Image)
                    {
                        Matrix pm = g.Transform.Clone();
                        Matrix nm = inst.GetTransformAtTime(0).Matrix.GetDrawing2DMatrix();
                        nm.Multiply(pm, MatrixOrder.Append);
                        g.Transform = nm;
                        DrawImage(stage.BitmapCache.GetBitmap(idef));
                        g.Transform = pm;
                    }
                }
            }
        }
        private void DrawSymbol(Vex.Symbol symbol)
        {
            for (int i = 0; i < symbol.Shapes.Count; i++)
            {
                Vex.Shape sh = symbol.Shapes[i];
                DrawShape(sh);
            }
        }
        private void DrawText(Vex.Text tx, DDW.Vex.Matrix m)
        {
            for (int i = 0; i < tx.TextRuns.Count; i++)
            {
                Vex.TextRun tr = tx.TextRuns[i];
                string s = tr.Text;
                FontStyle style = tr.isBold ? FontStyle.Bold : FontStyle.Regular;
                if (tr.isItalic)
                {
                    style |= FontStyle.Italic;
                }

                Font font = new Font(tr.FontName, tr.FontSize, style, GraphicsUnit.Pixel);

                System.Drawing.Color col = System.Drawing.Color.FromArgb(tr.Color.A, tr.Color.R, tr.Color.G, tr.Color.B);
                Brush b = new SolidBrush(col);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawString(s, font, b, tx.Matrix.TranslateX + m.TranslateX + tr.Left, tx.Matrix.TranslateY + m.TranslateY + tr.Top);

                b.Dispose();
            }
        }
        private void DrawShape(Vex.Shape sh)
        {
            List<GraphicsPath> paths;
            if (sh.Fill != null && fillOverride != Brushes.Transparent)
            {
                paths = GetPath(sh.ShapeData, true);
                FillPaths(sh.Fill, paths);
            }

            if (sh.Stroke != null)
            {
                if(penOverride != Pens.Transparent)
                {
                    paths = GetPath(sh.ShapeData, false);
                    StrokePaths(sh.Stroke, paths);
                }
            }
            else
            {
                // this gets rid of slight aliasing spaces between touching vectors
                // todo: do average colors for gradients or something.
                if (sh.Fill.FillType == Vex.FillType.Solid)
                {
                    Vex.StrokeStyle ss = new Vex.SolidStroke(.1F, ((Vex.SolidFill)sh.Fill).Color);
                    paths = GetPath(sh.ShapeData, false);
                    StrokePaths(ss, paths);
                }
            }
        }

        private void FillPaths(Vex.FillStyle fill, List<GraphicsPath> paths)
        {
            Brush b = null;
            foreach (GraphicsPath path in paths)
            {
                if (fillOverride != null)
                {
                    g.FillPath(fillOverride, path);
                }
                else
                {
                    switch (fill.FillType)
                    {
                        case Vex.FillType.Solid:
                            Vex.SolidFill sf = (Vex.SolidFill)fill;
                            b = new SolidBrush(sf.Color.SysColor());
                            break;

                        case Vex.FillType.Linear:
                            Vex.GradientFill lf = (Vex.GradientFill)fill;
                            RectangleF rect = Vex.GradientFill.GradientVexRect.SysRectangleF();
                            LinearGradientBrush lgb = new LinearGradientBrush(
                                rect,
                                Color.White,
                                Color.White,
                                1.0F
                                );
                            lgb.InterpolationColors = GetColorBlend(lf);
                            lgb.Transform = lf.Transform.SysMatrix();
                            lgb.WrapMode = WrapMode.TileFlipX;
                            ExtendGradientBrush(lgb, path);
                            b = lgb;
                            break;

                        case Vex.FillType.Radial:
                            Vex.GradientFill rf = (Vex.GradientFill)fill;

                            ColorBlend cb = GetColorBlend(rf);

                            SolidBrush bkgCol = new SolidBrush(cb.Colors[0]);
                            g.FillPath(bkgCol, path);
                            bkgCol.Dispose();

                            // radial fill part
                            GraphicsPath gp = new GraphicsPath();
                            gp.AddEllipse(Vex.GradientFill.GradientVexRect.SysRectangleF());

                            PathGradientBrush pgb = new PathGradientBrush(gp);
                            pgb.InterpolationColors = GetColorBlend(rf);
                            pgb.Transform = rf.Transform.SysMatrix();
                            b = pgb;
                            break;

                        case Vex.FillType.Image:
                            Vex.ImageFill imgFill = (Vex.ImageFill)fill;
                            Bitmap bmp = new Bitmap(imgFill.ImagePath);
                            b = new TextureBrush(bmp);
                            break;

                        default:
                            b = new SolidBrush(Color.Red);
                            break;
                    }
                    g.FillPath(b, path);
                }
            }

            if (b != null)
            {
                b.Dispose();
            }

        }
        private void StrokePaths(Vex.StrokeStyle stroke, List<GraphicsPath> paths)
        {
            Pen p = null;
            foreach (GraphicsPath path in paths)
            {
                if (penOverride != null)
                {
                    g.DrawPath(penOverride, path);
                }
                else
                {
                    if (stroke is Vex.SolidStroke)
                    {
                        Vex.SolidStroke ss = (Vex.SolidStroke)stroke;
                        p = new Pen(ss.Color.SysColor(), ss.LineWidth);
                    }
                    else
                    {
                        p = new Pen(Color.Black, 1);
                    }
                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;
                    p.LineJoin = LineJoin.Round;
                    g.DrawPath(p, path);
                    p.Dispose();
                }
            }
        }

        #region Utils
        public bool HasSymbols(Vex.Timeline tl)
        {
            bool result = false;
            for (int i = 0; i < tl.InstanceCount; i++)
            {
                Vex.IDefinition idef = stage.Library[tl.InstanceAt(i).DefinitionId].Definition;
                if (idef is Vex.Symbol)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        private Vex.Timeline GetNamedSymbol(Vex.Timeline tl)
        {
            Vex.Timeline result = null;

            if (tl.Name != "")
            {
                result = tl;
            }
            else
            {
                for (int i = 0; i < tl.InstanceCount; i++)
                {
                    Vex.IDefinition idef = stage.Library[tl.InstanceAt(i).DefinitionId].Definition;
                    if (idef is Vex.Timeline && ((Vex.Timeline)idef).Name != "")
                    {
                        result = (Vex.Timeline)idef;
                    }
                }
            }

            return result;
        }
        private List<GraphicsPath> GetPath(List<Vex.IShapeData> shapes, bool isFilled)
        {
            List<GraphicsPath> result = new List<GraphicsPath>();
            if (shapes.Count == 0)
            {
                return result;
            }
            DDW.Vex.Point endPoint = shapes[0].EndPoint;
            GraphicsPath gp = new GraphicsPath();
            gp.FillMode = FillMode.Alternate;
            result.Add(gp);

            for (int i = 0; i < shapes.Count; i++)
            {
                Vex.IShapeData sd = shapes[i];

                if (sd.StartPoint != endPoint)
                {
                    if (isFilled)
                    {
                        gp.CloseFigure();
                    }
                    else
                    {
                        gp = new GraphicsPath();
                        gp.FillMode = FillMode.Alternate;
                        result.Add(gp);
                    }
                }
                switch (sd.SegmentType)
                {
                    case Vex.SegmentType.Line:
                        Vex.Line l = (Vex.Line)sd;
                        gp.AddLine(l.Anchor0.X, l.Anchor0.Y, l.Anchor1.X, l.Anchor1.Y);
                        break;
                    case Vex.SegmentType.CubicBezier:
                        Vex.CubicBezier cb = (Vex.CubicBezier)sd;
                        gp.AddBezier(
                            cb.Anchor0.X, cb.Anchor0.Y,
                            cb.Control0.X, cb.Control0.Y,
                            cb.Control1.X, cb.Control1.Y,
                            cb.Anchor1.X, cb.Anchor1.Y);
                        break;

                    case Vex.SegmentType.QuadraticBezier:
                        Vex.QuadBezier qb = (Vex.QuadBezier)sd;
                        Vex.CubicBezier qtc = qb.GetCubicBezier();
                        gp.AddBezier(
                            qtc.Anchor0.X, qtc.Anchor0.Y,
                            qtc.Control0.X, qtc.Control0.Y,
                            qtc.Control1.X, qtc.Control1.Y,
                            qtc.Anchor1.X, qtc.Anchor1.Y);
                        break;
                }
                endPoint = sd.EndPoint;
            }
            if (isFilled)
            {
                gp.CloseFigure();
            }
            return result;
        }
        private ColorBlend GetColorBlend(Vex.GradientFill fill)
        {
            List<float> positions = new List<float>();
            List<Color> colors = new List<Color>();

            int numGradients = fill.Fills.Count;

            for (int i = 0; i < numGradients; i++)
            {
                positions.Add(fill.Stops[i]);
                colors.Add(fill.Fills[i].SysColor());
            }

            // GDI color blends must start at 0.0 and end at 1.0 or they will crash
            if ((float)positions[0] != 0.0F)
            {
                positions.Insert(0, 0.0F);
                colors.Insert(0, colors[0]);
            }
            if ((float)positions[positions.Count - 1] != 1.0F)
            {
                positions.Add(1.0F);
                colors.Add(colors[colors.Count - 1]);
            }

            ColorBlend cb = new ColorBlend(positions.Count);
            cb.Colors = colors.ToArray();
            cb.Positions = positions.ToArray();

            return cb;
        }
        private Bitmap CreateRenderBitmap(Vex.Rectangle bounds, Matrix m)
        {
            if (g != null)
            {
                g.Dispose();
            }
            RectangleF sysBounds = bounds.SysRectangleF();
            SizeF absSize = GetAbsTransformedSize(sysBounds, m);

            Bitmap img = new Bitmap(
                (int)absSize.Width + 1,
                (int)absSize.Height + 1);

            img.SetResolution(96, 96);

            g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.AssumeLinear;

            g.Transform = m;

            return img;
        }
        #endregion

        #region Static Utils
        public static void TransformPoints(PointF[] pts, Matrix m, bool invert)
        {
            if (invert)
            {
                m.Invert();
                m.TransformPoints(pts);
                m.Invert();
            }
            else
            {
                m.TransformPoints(pts);
            }
        }
        public static RectangleF GetTransformedBounds(Vex.IDefinition def, Matrix m, bool invert)
        {
            PointF[] pts = def.StrokeBounds.SysPointFs();
            TransformPoints(pts, m, invert);
            return pts.GetBounds();
        }
        public static RectangleF GetTransformedBounds(RectangleF r, Matrix m, bool invert)
        {
            PointF[] pts = r.Points();
            TransformPoints(pts, m, invert);
            return pts.GetBounds();
        }
        public static PointF GetTransformedPoint(PointF pt, Matrix m, bool invert)
        {
            PointF[] pts = new PointF[] { pt };
            TransformPoints(pts, m, invert);
            return pts[0];
        }
        public static Vex.Point GetTransformedPoint(Vex.Point pt, Vex.Matrix m, bool invert)
        {
            PointF[] pts = new PointF[] { pt.SysPointF() };
            using (Matrix ms = m.SysMatrix())
            {
                TransformPoints(pts, ms, invert);
            }
            return pts[0].VexPoint();
        }
        private static SizeF GetAbsTransformedSize(RectangleF r, Matrix m)
        {
            PointF[] pts = new PointF[] { new PointF(r.Width, r.Height) };
            Matrix ms = new Matrix(m.Elements[0], 0, 0, m.Elements[3], 0, 0);
            ms.TransformPoints(pts);
            return new SizeF(Math.Abs(pts[0].X), Math.Abs(pts[0].Y));
        }
        public static Bitmap GetEmptyBitmap(SizeF size)
        {
            Bitmap result = new Bitmap(
                (int)Math.Ceiling(size.Width),
                (int)Math.Ceiling(size.Height));

            result.SetResolution(96, 96);

            return result;
        }
        public static Graphics GetGraphicsFromBitmap(Bitmap bmp)
        {
            bmp.SetResolution(96, 96);

            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.AssumeLinear;

            return g;
        }
        private static LinearGradientBrush ExtendGradientBrush(LinearGradientBrush brush, GraphicsPath path)
        {
            // get the untransformed gradient rectangle
            RectangleF gradRect = brush.Rectangle;
            // put it into a points array starting with top right
            PointF[] gradPoints = new PointF[4]
					{ 
						new PointF(gradRect.Right, gradRect.Top), 
						new PointF(gradRect.Right, gradRect.Bottom),
						new PointF(gradRect.Left,  gradRect.Bottom),
						new PointF(gradRect.Left,  gradRect.Top)
					};
            // transform the points to get the two edges of the gradient as 
            // tr-br and bl-tl. The width of the gradient can be found at the bottom.
            // This makes it easier to figure out which corners need to be tested.
            brush.Transform.TransformPoints(gradPoints);

            RectangleF pathRect = path.GetBounds();

            // find the corner point to test to see if it might be past the gradient
            // first make the forward(AfBf) and back(AbBb) edge lines of the gradient
            PointF Af = gradPoints[0];
            PointF Bf = gradPoints[1];
            PointF Ab = gradPoints[2];
            PointF Bb = gradPoints[3];

            // set forward and back test corner
            PointF Cb = pathRect.Location;
            PointF Cf = pathRect.Location;
            if (Af.X >= Bf.X)
            {
                Cf.Y += pathRect.Height;
            }
            else
            {
                Cb.Y += pathRect.Height;
            }
            if (Af.Y < Bf.Y)
            {
                Cf.X += pathRect.Width;
            }
            else
            {
                Cb.X += pathRect.Width;
            }
            // gradient width is the connection lines if grad isn't skewed (same for both)
            // check if gradients can ever be skewed... if so, calc line to line dist.
            float gradW = (float)Math.Sqrt(
                (Bf.X - Ab.X) * (Bf.X - Ab.X) + (Bf.Y - Ab.Y) * (Bf.Y - Ab.Y));
            // length of gradient edge (same for both sides)
            float gradH = (float)Math.Sqrt(
                (Bf.X - Af.X) * (Bf.X - Af.X) + (Bf.Y - Af.Y) * (Bf.Y - Af.Y));

            // now check if the path data might be bigger than the gradient
            int hasFRatio = 0;
            int hasBRatio = 0;
            // in the forward direction 
            float distToLineF = 0;
            float distToLineB = 0;
            float sf = ((Af.Y - Cf.Y) * (Bf.X - Af.X) - (Af.X - Cf.X) * (Bf.Y - Af.Y)) / (gradH * gradH);
            if (sf > 0)
            {
                // graphic may be bigger than fill so 
                // figure out how much bigger the fill has to be 
                // (meaning how much smaller the original gradient must be)
                distToLineF = Math.Abs(sf) * gradH;
                hasFRatio = 1;
            }
            // in the back direction 
            float sb = ((Ab.Y - Cb.Y) * (Bb.X - Ab.X) - (Ab.X - Cb.X) * (Bb.Y - Ab.Y)) / (gradH * gradH);
            if (sb > 0)
            {
                distToLineB = Math.Abs(sb) * gradH; ;
                hasBRatio = 1;
            }

            // Now we have the info we need to tell if the gradient doesn't fit in the path
            if ((hasFRatio + hasBRatio) > 0)
            {
                float totalNewWidth = distToLineF + distToLineB + gradW;
                float ratioB = distToLineB / totalNewWidth;
                float ratioF = distToLineF / totalNewWidth;
                float compressRatio = gradW / totalNewWidth;
                float expandRatio = totalNewWidth / gradW; // eg. 1/compressRatio

                float[] pos = brush.InterpolationColors.Positions;
                float[] newPos = new float[pos.Length + hasFRatio + hasBRatio];
                Color[] cols = brush.InterpolationColors.Colors;
                Color[] newCols = new Color[cols.Length + hasFRatio + hasBRatio];
                if (hasBRatio == 1)
                {
                    newPos[0] = 0;
                    newCols[0] = cols[0];
                }
                for (int i = 0; i < pos.Length; i++)
                {
                    newPos[i + hasBRatio] = pos[i] * compressRatio + ratioB;
                    newCols[i + hasBRatio] = cols[i];
                }
                newPos[newPos.Length - 1] = 1;
                newCols[newCols.Length - 1] = cols[cols.Length - 1];

                ColorBlend cb2 = new ColorBlend(newPos.Length);
                cb2.Positions = newPos;
                cb2.Colors = newCols;
                brush.InterpolationColors = cb2;

                System.Drawing.Drawing2D.Matrix m2 = brush.Transform;
                // scale it with the edge at the orgin
                m2.Translate(-Bb.X, -Bb.Y, MatrixOrder.Append);
                m2.Scale(expandRatio, expandRatio, MatrixOrder.Append);
                // now move it back to be on the back edge, whatever that is
                if (hasBRatio == 1)
                {
                    m2.Translate(Cb.X, Cb.Y, MatrixOrder.Append);
                }
                else
                {
                    m2.Translate(Bb.X, Bb.Y, MatrixOrder.Append);
                }
                brush.Transform = m2;
            }
            return brush;
        }
        #endregion
    }
}
