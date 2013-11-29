using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sysDraw = System.Drawing;
using sysDraw2D = System.Drawing.Drawing2D;
using DDW.Vex;

namespace DDW.VectorML
{
    public abstract class XamlBaseRenderer : MLRenderer
    {
        protected override void RenderPath(FillStyle fs, StrokeStyle ss, List<IShapeData> sh, bool silverlight)
        {
            // <Path Fill="#FFFF0000" 
            // StrokeThickness="0.00491913" StrokeLineJoin="Round" Stroke="#FF014393"
            // Data="M 196.667,4L 388.667,100L 388.667,292L 196.667,388L 4.66669,292L 4.66669,100L 196.667,4 Z "/>
            if (sh.Count == 0)
            {
                return;
            }

            xw.WriteStartElement("Path");

            bool isGradient = false;
            bool isTiledBitmap = false;

            if (fs != null)
            {
                if (fs.FillType == FillType.Solid)
                {
                    Color c = ((SolidFill)fs).Color;
                    xw.WriteStartAttribute("Fill");
                    xw.WriteColor(c);
                    xw.WriteEndAttribute();

                    // try to clean up faint edges
                    if (ss == null && c != new Color(0xFF, 0xFF, 0xFF) && c.A != 0)
                    {
                        ss = new SolidStroke(0.3F, c);
                    }
                }
                else if (
                    fs.FillType == FillType.Linear ||
                    fs.FillType == FillType.Radial ||
                    fs.FillType == FillType.Focal)
                {
                    isGradient = true;
                }
                else if (fs.FillType == FillType.Image)
                {
                    // Fill="{StaticResource vb_1}" 
                    ImageFill img = (ImageFill)fs;
                    if (img.IsTiled || silverlight)
                    {
                        isTiledBitmap = true;// this causes bitmap to be written inline
                    }
                    else
                    {
                        string brushName = imageBrushes[img.ImagePath];
                        xw.WriteStartAttribute("Fill");
                        xw.WriteValue("{StaticResource " + brushName + "}");
                        xw.WriteEndAttribute();
                    }
                }
            }
            if (ss != null)
            {
                if (ss is SolidStroke)
                {
                    // StrokeThickness="3" StrokeLineJoin="Round" Stroke="#FF014393"
                    // StrokeStartLineCap="Round"
                    // StrokeEndLineCap="Round" 
                    SolidStroke st = (SolidStroke)ss;

                    xw.WriteStartAttribute("StrokeThickness");
                    xw.WriteFloat(st.LineWidth);
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("StrokeLineJoin");
                    xw.WriteString("Round");
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("StrokeStartLineCap");
                    xw.WriteString("Round");
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("StrokeEndLineCap");
                    xw.WriteString("Round");
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("Stroke");
                    xw.WriteColor(st.Color);
                    xw.WriteEndAttribute();
                }
            }
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            xw.WriteStartAttribute("Data");

            xw.WriteMoveTo(sh[0].StartPoint);

            Point lastPoint = sh[0].StartPoint;

            for (int i = 0; i < sh.Count; i++)
            {
                IShapeData sd = sh[i];
                if (lastPoint != sd.StartPoint)
                {
                    xw.WriteMoveTo(sd.StartPoint);
                }
                switch (sd.SegmentType)
                {
                    case SegmentType.Line:
                        xw.WriteLineTo(sd.EndPoint);
                        lastPoint = sd.EndPoint;
                        break;

                    case SegmentType.CubicBezier:
                        CubicBezier cb = (CubicBezier)sd;
                        xw.WriteCubicCurveTo(cb.Control0, cb.Control1, cb.Anchor1);
                        lastPoint = cb.EndPoint;
                        minX = Math.Min(minX, cb.Control0.X);
                        maxX = Math.Max(maxX, cb.Control0.X);
                        minY = Math.Min(minY, cb.Control0.Y);
                        maxY = Math.Max(maxY, cb.Control0.Y);
                        minX = Math.Min(minX, cb.Control1.X);
                        maxX = Math.Max(maxX, cb.Control1.X);
                        minY = Math.Min(minY, cb.Control1.Y);
                        maxY = Math.Max(maxY, cb.Control1.Y);
                        break;

                    case SegmentType.QuadraticBezier:
                        QuadBezier qb = (QuadBezier)sd;
                        xw.WriteQuadraticCurveTo(qb.Control, qb.Anchor1);
                        lastPoint = qb.EndPoint;
                        minX = Math.Min(minX, qb.Control.X);
                        maxX = Math.Max(maxX, qb.Control.X);
                        minY = Math.Min(minY, qb.Control.Y);
                        maxY = Math.Max(maxY, qb.Control.Y);
                        break;
                }

                // need bounds for gradient :(
                if (isGradient)
                {
                    minX = Math.Min(minX, sd.StartPoint.X);
                    maxX = Math.Max(maxX, sd.StartPoint.X);
                    minY = Math.Min(minY, sd.StartPoint.Y);
                    maxY = Math.Max(maxY, sd.StartPoint.Y);

                    minX = Math.Min(minX, sd.EndPoint.X);
                    maxX = Math.Max(maxX, sd.EndPoint.X);
                    minY = Math.Min(minY, sd.EndPoint.Y);
                    maxY = Math.Max(maxY, sd.EndPoint.Y);
                }
            }
            xw.WriteEndAttribute();

            if (isGradient)
            {
                GradientFill gf = (GradientFill)fs;
                // need a gradient def here
                if (fs.FillType == FillType.Linear)
                {
                    //<Path.Fill>
                    //    <LinearGradientBrush StartPoint="0.14706,0.532137" EndPoint="1.14962,0.55353">
                    //        <LinearGradientBrush.GradientStops>
                    //            <GradientStop Color="#FF4A4A4A" Offset="0"/>
                    //            <GradientStop Color="#FFB0B0B0" Offset="0.412067"/>
                    //            <GradientStop Color="#FFBBBBBB" Offset="0.638141"/>
                    //            <GradientStop Color="#FF545454" Offset="1"/>
                    //        </LinearGradientBrush.GradientStops>
                    //    </LinearGradientBrush>
                    //</Path.Fill>
                    xw.WriteStartElement("Path.Fill");

                    xw.WriteStartElement("LinearGradientBrush");

                    Matrix m = gf.Transform;
                    Rectangle r = GradientFill.GradientVexRect;
                    sysDraw2D.Matrix m2 = new sysDraw2D.Matrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, m.TranslateX, m.TranslateY);
                    float midY = r.Point.Y + (r.Size.Height / 2);
                    sysDraw.PointF pt0 = new sysDraw.PointF(r.Point.X, midY);
                    sysDraw.PointF pt1 = new sysDraw.PointF(r.Point.X + r.Size.Width, midY);
                    sysDraw.PointF[] pts = new sysDraw.PointF[] { pt0, pt1 };
                    m2.TransformPoints(pts);

                    float ratX = 1 / (maxX - minX);
                    float ratY = 1 / (maxY - minY);
                    float d0x = (pts[0].X - minX) * ratX;
                    float d0y = (pts[0].Y - minY) * ratY;
                    float d1x = (pts[1].X - minX) * ratX;
                    float d1y = (pts[1].Y - minY) * ratY;

                    xw.WriteStartAttribute("StartPoint");
                    xw.WritePoint(new Point(d0x, d0y));
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("EndPoint");
                    xw.WritePoint(new Point(d1x, d1y));
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("SpreadMethod");
                    xw.WriteValue("Pad");
                    xw.WriteEndAttribute();

                    xw.WriteStartElement("LinearGradientBrush.GradientStops");
                    for (int i = 0; i < gf.Stops.Count; i++)
                    {
                        xw.WriteStartElement("GradientStop");

                        xw.WriteStartAttribute("Color");
                        xw.WriteColor(gf.Fills[i]);
                        xw.WriteEndAttribute();

                        xw.WriteStartAttribute("Offset");
                        xw.WriteFloat(gf.Stops[i]);
                        xw.WriteEndAttribute();

                        xw.WriteEndElement(); // GradientStop
                    }
                    xw.WriteEndElement(); // LinearGradientBrush.GradientStops
                    xw.WriteEndElement(); // LinearGradientBrush
                    xw.WriteEndElement(); // Path.Fill
                }
                else if (fs.FillType == FillType.Radial)
                {
                    //<Ellipse.Fill>
                    //    <RadialGradientBrush RadiusX="0.622359" RadiusY="0.604589" Center="0.5,0.5" GradientOrigin="0.5,0.5">
                    //        <RadialGradientBrush.RelativeTransform>
                    //            <TransformGroup/>
                    //        </RadialGradientBrush.RelativeTransform>
                    //        <GradientStop Color="#95000000" Offset="0.347222"/>
                    //        <GradientStop Color="#007877A7" Offset="0.773148"/>
                    //    </RadialGradientBrush>
                    //</Ellipse.Fill>

                    xw.WriteStartElement("Path.Fill");

                    xw.WriteStartElement("RadialGradientBrush");

                    Matrix m = gf.Transform;
                    Rectangle r = GradientFill.GradientVexRect;
                    sysDraw2D.Matrix m2 = new sysDraw2D.Matrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, m.TranslateX, m.TranslateY);
                    float midX = r.Point.X + (r.Size.Width / 2);
                    float midY = r.Point.Y + (r.Size.Height / 2);
                    sysDraw.PointF pt0 = new sysDraw.PointF(midX, midY); // center
                    sysDraw.PointF pt1 = new sysDraw.PointF(r.Point.X + r.Size.Width, midY); // radius vector
                    sysDraw.PointF[] pts = new sysDraw.PointF[] { pt0, pt1 };
                    m2.TransformPoints(pts);

                    float ratX = 1 / (maxX - minX);
                    float ratY = 1 / (maxY - minY);
                    float d0x = (pts[0].X - minX) * ratX;
                    float d0y = (pts[0].Y - minY) * ratY;
                    float d1x = (pts[1].X - pts[0].X);
                    //float d1y = (pts[1].Y - pts[0].Y) * ratY;

                    float rad = (float)Math.Sqrt(d1x * d1x);
                    xw.WriteStartAttribute("RadiusX");
                    xw.WriteFloat(rad * ratX);
                    xw.WriteEndAttribute();
                    xw.WriteStartAttribute("RadiusY");
                    xw.WriteFloat(rad * ratY);
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("Center");
                    xw.WritePoint(new Point(d0x, d0y));
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("GradientOrigin");
                    xw.WritePoint(new Point(d0x, d0y));
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("SpreadMethod");
                    xw.WriteValue("Pad");
                    xw.WriteEndAttribute();

                    //xw.WriteStartElement("RadialGradientBrush.GradientStops");
                    for (int i = 0; i < gf.Stops.Count; i++)
                    {
                        xw.WriteStartElement("GradientStop");

                        xw.WriteStartAttribute("Color");
                        xw.WriteColor(gf.Fills[i]);
                        xw.WriteEndAttribute();

                        xw.WriteStartAttribute("Offset");
                        xw.WriteFloat(1 - gf.Stops[i]); // xaml fill is reversed from gdi
                        xw.WriteEndAttribute();

                        xw.WriteEndElement(); // GradientStop
                    }
                    //xw.WriteEndElement(); // LinearGradientBrush.GradientStops
                    xw.WriteEndElement(); // LinearGradientBrush
                    xw.WriteEndElement(); // Path.Fill
                }
            }
            else if (isTiledBitmap)
            {
                //<Path.Fill>
                //   <ImageBrush ImageSource="Resources\bmp_1.jpg" TileMode="Tile" RelativeTransform=".2,0,0,.2,0,0"/>
                //</Path.Fill>  

                ImageFill img = (ImageFill)fs;

                xw.WriteStartElement("Path.Fill");
                xw.WriteStartElement("ImageBrush");

                xw.WriteStartAttribute("ImageSource");
                xw.WriteValue(img.ImagePath);
                xw.WriteEndAttribute();

                if (!silverlight)
                {
                    xw.WriteStartAttribute("TileMode");
                    xw.WriteValue("Tile");
                    xw.WriteEndAttribute();
                }

                //xw.WriteStartAttribute("ViewportUnits");
                //xw.WriteValue("Absolute");
                //xw.WriteEndAttribute();

                Matrix pMatrix = ApplyMatrixToShape(sh, img.Matrix, images[img.ImagePath].StrokeBounds);
                //Matrix pMatrix = ApplyMatrixToImage(img.Matrix, images[img.ImagePath].Bounds);
                xw.WriteStartAttribute("RelativeTransform");
                xw.WriteMatrix(pMatrix);
                //xw.WriteMatrix(img.Matrix);
                xw.WriteEndAttribute();

                xw.WriteEndElement(); // Path.Fill
                xw.WriteEndElement(); // ImageBrush
            }
            xw.WriteEndElement(); // Path

        }


        protected override void WriteTransformDef(Instance inst, string fullName)
        {
            //<Canvas.RenderTransform>            
            //  <MatrixTransform x:Name="xxxx">
            //  <MatrixTransform.Matrix >
            //      <Matrix OffsetX="10" OffsetY="100" M11="3" M12="2"/>
            //  </MatrixTransform.Matrix>
            //  </MatrixTransform>
            //</Canvas.RenderTransform>              

            Matrix m = inst.Transformations[0].Matrix;

            bool hasTranslate = (m.Location != Point.Zero);
            bool hasAffine = (m.ScaleX != 1) || (m.Rotate0 != 0) || (m.Rotate1 != 0) || (m.ScaleY != 1);

            if (hasTranslate || hasAffine)
            {
                xw.WriteStartElement("Canvas.RenderTransform");

                xw.WriteStartElement("MatrixTransform");
                xw.WriteStartAttribute("x:Name");
                xw.WriteValue("mx_" + fullName);
                xw.WriteEndAttribute();

                xw.WriteStartElement("MatrixTransform.Matrix");

                xw.WriteStartElement("Matrix");

                if (hasTranslate)
                {
                    xw.WriteStartAttribute("OffsetX");
                    xw.WriteValue(m.Location.X);
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("OffsetY");
                    xw.WriteValue(m.Location.Y);
                    xw.WriteEndAttribute();
                }

                if (hasAffine)
                {
                    xw.WriteStartAttribute("M11");
                    xw.WriteValue(m.ScaleX);
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("M12");
                    xw.WriteValue(m.Rotate0);
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("M21");
                    xw.WriteValue(m.Rotate1);
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("M22");
                    xw.WriteValue(m.ScaleY);
                    xw.WriteEndAttribute();
                }

                xw.WriteEndElement(); //Matrix
                xw.WriteEndElement(); //MatrixTransform.Matrix
                xw.WriteEndElement(); //MatrixTransform
                xw.WriteEndElement(); //Canvas.RenderTransform
            }
        }
        //protected void WriteTransformsDefs(IDefinition s, Instance inst, string fullName)
        //{
        //    //<Canvas.RenderTransform>
        //    //<TransformGroup>
        //    //  <RotateTransform x:Name="inst_rt_0r" Angle="0" />
        //    //  <SkewTransform x:Name="inst_rt_0k" AngleX="0" AngleY="0"/>
        //    //  <ScaleTransform x:Name="inst_rt_0s" ScaleX="1" ScaleY="1" />
        //    //  <TranslateTransform x:Name="inst_rt_0t" X="35" Y="26" />
        //    //</TransformGroup>
        //    //</Canvas.RenderTransform>  

        //    IDefinition def = v.Definitions[s.Id];
        //    //string defName = instancePrefix + GetInstanceName(inst);

        //    Rectangle r = s.StrokeBounds;
        //    Matrix m = inst.Transformations[0].Matrix;
        //    MatrixComponents mt = m.GetMatrixComponents();

        //    bool multiTransform = true;// inst.Transformations.Count > 1;
        //    bool hasRot = !(mt.Rotation == 0);
        //    bool hasSkew = !(mt.Shear == 0);
        //    bool hasScale = !(mt.ScaleX == 1 && mt.ScaleY == 1);
        //    bool hasTranslate = !(mt.TranslateX == 0 && mt.TranslateY == 0 && r.Point.X == 0 && r.Point.Y == 0);

        //    if (multiTransform || hasRot || hasSkew || hasScale || hasTranslate)
        //    {
        //        xw.WriteStartElement("Canvas.RenderTransform");
        //        xw.WriteStartElement("TransformGroup");

        //        if (multiTransform || hasRot)
        //        {
        //            xw.WriteStartElement("RotateTransform");
        //            xw.WriteStartAttribute("x:Name");
        //            xw.WriteValue(fullName + "r");
        //            xw.WriteEndAttribute();
        //            xw.WriteStartAttribute("Angle");
        //            //xw.WriteValue(mt.Rotation);
        //            xw.WriteValue(0);
        //            xw.WriteEndAttribute();
        //            xw.WriteEndElement();
        //        }

        //        if (multiTransform || hasSkew)
        //        {
        //            xw.WriteStartElement("SkewTransform");
        //            xw.WriteStartAttribute("x:Name");
        //            xw.WriteValue(fullName + "k");
        //            xw.WriteEndAttribute();
        //            xw.WriteStartAttribute("AngleX");
        //            //xw.WriteValue(mt.Shear);
        //            xw.WriteValue(0);
        //            xw.WriteEndAttribute();
        //            xw.WriteStartAttribute("AngleY");
        //            xw.WriteValue(0);
        //            xw.WriteEndAttribute();
        //            xw.WriteEndElement();
        //        }

        //        if (multiTransform || hasScale)
        //        {
        //            xw.WriteStartElement("ScaleTransform");
        //            xw.WriteStartAttribute("x:Name");
        //            xw.WriteValue(fullName + "s");
        //            xw.WriteEndAttribute();
        //            xw.WriteStartAttribute("ScaleX");
        //            //xw.WriteValue(mt.ScaleX);
        //            xw.WriteValue(1);
        //            xw.WriteEndAttribute();
        //            xw.WriteStartAttribute("ScaleY");
        //            //xw.WriteValue(mt.ScaleY);
        //            xw.WriteValue(1);
        //            xw.WriteEndAttribute();
        //            xw.WriteEndElement();
        //        }

        //        if (multiTransform || hasTranslate)
        //        {
        //            xw.WriteStartElement("TranslateTransform");
        //            xw.WriteStartAttribute("x:Name");
        //            xw.WriteValue(fullName + "t");
        //            xw.WriteEndAttribute();
        //            xw.WriteStartAttribute("X");
        //            //xw.WriteValue(mt.TranslateX + r.Point.X);
        //            xw.WriteValue(0);
        //            xw.WriteEndAttribute();
        //            xw.WriteStartAttribute("Y");
        //            //xw.WriteValue(mt.TranslateY + r.Point.Y);
        //            xw.WriteValue(0);
        //            xw.WriteEndAttribute();
        //            xw.WriteEndElement();
        //        }

        //        xw.WriteEndElement(); //TransformGroup
        //        xw.WriteEndElement(); //Canvas.RenderTransform
        //    }
        //}

    }
}
