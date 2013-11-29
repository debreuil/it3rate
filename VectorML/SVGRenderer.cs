using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;
using System.IO;
using sysDraw = System.Drawing;
using sysDraw2D = System.Drawing.Drawing2D;

namespace DDW.VectorML
{
    public class SVGRenderer : MLRenderer
    {
        public override void GenerateML(VexObject v, string path, out string mlFileName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.v = v;
            this.Log = new StringBuilder();
            mlFileName = path + v.Name + ".svg";
            xw = new SvgWriter(mlFileName, Encoding.UTF8);

            xw.WriteComment(headerComment);

            xw.OpenHeaderTag(v.ViewPort.Size.Width, v.ViewPort.Size.Height, v.BackgroundColor);

            WriteDefinitions(v.Definitions, true, false);

            WriteTimelineDefiniton(v.Root, true);

            xw.CloseTagAndFlush();

            xw.Close();
        }
        public override void GenerateMLPart(VexObject v, IDefinition def, out string fileName)
        {
            this.v = v;
            this.Log = new StringBuilder();
            fileName = Directory.GetCurrentDirectory() + "/" + v.Name + "_" + def.Id + ".svg";
            xw = new XamlWriter(fileName, Encoding.UTF8);

            xw.WriteComment(headerComment);

            xw.OpenHeaderTag(def.StrokeBounds.Size.Width, def.StrokeBounds.Size.Height, v.BackgroundColor);

            Dictionary<uint, IDefinition> defList = new Dictionary<uint, IDefinition>();
            defList.Add(1, def);
            WriteDefinitions(defList, true, true);

            //WriteTimelineDefiniton(v.Root, true);
            // Write a rectangle to hold this shape
            Instance inst = new Instance();
            inst.Name = instancePrefix + def.Id;
            inst.InstanceHash = 1;
            inst.DefinitionId = def.Id;
            inst.Transformations.Add(new Transform(0, 1000, Matrix.Identity, 1, ColorTransform.Identity));
            WriteInstance(def, inst);

            xw.CloseTagAndFlush();

            xw.Close();
        }

        public void WriteDefinitions(Dictionary<uint, IDefinition> defs, bool isRoot, bool insurePartialImages)
        {
            xw.OpenResourcesTag();
            if (insurePartialImages)
            {
                AddImagesPart(defs, isRoot);
            }
            // write defs
            foreach (uint key in defs.Keys)
            {
                IDefinition def = defs[key];
                if (def is Symbol)
                {
                    WriteSymbolDefinition((Symbol)def);
                }
                else if (def is Timeline)
                {
                    //WriteTimelineDefiniton((Timeline)def, false);
                }
                else if (def is Image)
                {
                    DefineImage((Image)def);
                }
                else if (def is Text)
                {
                    // text is inlined
                }
                else
                {
                    if (def != null)
                    {
                        Console.WriteLine("Non supported Vex element in SVG: " + def.GetType());
                    }
                }
            }
            // write watermark
            if (isWatermarking && isRoot)
            {
                WriteWatermarkDefinitions();
            }
            xw.CloseTagAndFlush();
        }
        public override void WriteTimelineDefiniton(Timeline timeline, bool isRoot)
        {
            if (isRoot)
            {
                xw.OpenRootTag();
            }
            else
            {
                xw.OpenTimelineTag(instancePrefix + instName);//timelinePrefix + timeline.Id);
                Instance inst = timelineStack.Peek();
                WriteTransformDef(inst, instName);
            }

            WriteInstances(timeline.Instances, isRoot);

            xw.CloseTag();
        }

        public override void WriteInstance(IDefinition s, Instance inst)
        {
            //<use id="inst1" xlink:href="#example"  width="100" height="100" x="5" y="5"/>

            xw.WriteStartElement("use");

            xw.WriteStartAttribute("id");
            xw.WriteValue(instancePrefix + instName);
            xw.WriteEndAttribute();

            string prefix = v.Definitions[inst.DefinitionId] is Image ? imageBrushPrefix : symbolPrefix;
            xw.WriteStartAttribute("xlink:href");
            xw.WriteString("#" + prefix + inst.DefinitionId);
            xw.WriteEndAttribute();

            xw.WriteStartAttribute("width");
            xw.WriteFloat(s.StrokeBounds.Size.Width);
            xw.WriteEndAttribute();

            xw.WriteStartAttribute("height");
            xw.WriteFloat(s.StrokeBounds.Size.Height);
            xw.WriteEndAttribute();

            //RenderTransformOrigin="1,1" 
            // bug from yahoo, width and hieght can be zero, when line with no linestyle is applied
            float tx = s.StrokeBounds.Size.Width == 0 ?
                -s.StrokeBounds.Point.X :
                -s.StrokeBounds.Point.X / s.StrokeBounds.Size.Width;
            float ty = s.StrokeBounds.Size.Height == 0 ?
                -s.StrokeBounds.Point.Y :
                -s.StrokeBounds.Point.Y / s.StrokeBounds.Size.Height;

            if (tx != 0)
            {
                xw.WriteStartAttribute("x");
                xw.WriteFloat(tx);
                xw.WriteEndAttribute();
            }

            if (tx != 0)
            {
                xw.WriteStartAttribute("y");
                xw.WriteFloat(ty);
                xw.WriteEndAttribute();
            }

            xw.WriteEndElement();
        }
        public override void WriteSoundInstance(SoundInstance sound)
        {
            //<MediaElement Name="sndInst_0"/>

            xw.WriteStartElement("MediaElement");

            xw.WriteStartAttribute("Name");
            xw.WriteValue(VexObject.SoundPrefix + sound.DefinitionId);
            xw.WriteEndAttribute();

            xw.WriteEndElement();
        }
        public override void WriteSymbolDefinition(Symbol symbol)
        {
            string defName = GetDefinitionName(symbol);

            List<Shape> gradientShapes = new List<Shape>();
            for (int i = 0; i < symbol.Shapes.Count; i++)
            {
                if (symbol.Shapes[i].Fill is GradientFill)
                {
                    gradientShapes.Add(symbol.Shapes[i]);
                }
            }
            foreach (Shape shape in gradientShapes)
            {
                WriteGradientDefinition(shape);
            }

            xw.OpenSymbolDefTag(defName, (int)symbol.StrokeBounds.Size.Width, (int)symbol.StrokeBounds.Size.Height);
            for (int i = 0; i < symbol.Shapes.Count; i++)
            {
                Shape sh = symbol.Shapes[i];
                RenderPath(sh.Fill, sh.Stroke, sh.ShapeData, false);
            }
            xw.CloseTag();
        }
        private uint gradientCounter = 0;
        public void WriteGradientDefinition(Shape shape)
        {
            GradientFill gf = (GradientFill)shape.Fill;
            gf.TagId = gradientCounter++;

            List<IShapeData> shapeData = shape.ShapeData;
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            Point lastPoint = shapeData[0].StartPoint;

            for (int i = 0; i < shapeData.Count; i++)
            {
                IShapeData sd = shapeData[i];
                switch (sd.SegmentType)
                {
                    case SegmentType.Line:
                        lastPoint = sd.EndPoint;
                        break;

                    case SegmentType.CubicBezier:
                        CubicBezier cb = (CubicBezier)sd;
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
                        lastPoint = qb.EndPoint;
                        minX = Math.Min(minX, qb.Control.X);
                        maxX = Math.Max(maxX, qb.Control.X);
                        minY = Math.Min(minY, qb.Control.Y);
                        maxY = Math.Max(maxY, qb.Control.Y);
                        break;
                }

                minX = Math.Min(minX, sd.StartPoint.X);
                maxX = Math.Max(maxX, sd.StartPoint.X);
                minY = Math.Min(minY, sd.StartPoint.Y);
                maxY = Math.Max(maxY, sd.StartPoint.Y);

                minX = Math.Min(minX, sd.EndPoint.X);
                maxX = Math.Max(maxX, sd.EndPoint.X);
                minY = Math.Min(minY, sd.EndPoint.Y);
                maxY = Math.Max(maxY, sd.EndPoint.Y);
            }


            if (gf.FillType == FillType.Linear)
            {
                //<linearGradient id = "g1" x1 = "50%" y1 = "50%" x2 = "60%" y2 = "60%">
                //    <stop stop-color = "green" offset = "0%"/>
                //    <stop stop-color = "pink" offset = "100%"/>
                //</linearGradient>

                xw.WriteStartElement("linearGradient");

                xw.WriteStartAttribute("id");
                xw.WriteValue("gf_" + gf.TagId);
                xw.WriteEndAttribute();

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
                
                xw.WriteStartAttribute("x1");
                xw.WriteFloat(d0x);
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("y1");
                xw.WriteFloat(d0y);
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("x2");
                xw.WriteFloat(d1x);
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("y2");
                xw.WriteFloat(d1y);
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("spreadMethod");
                xw.WriteValue("pad");
                xw.WriteEndAttribute();

                for (int i = 0; i < gf.Stops.Count; i++)
                {
                    xw.WriteStartElement("stop");

                    xw.WriteStartAttribute("stop-color");
                    xw.WriteValue("#" + gf.Fills[i].RGB.ToString("X6"));
                    xw.WriteEndAttribute();

                    if (gf.Fills[i].A < 255)
                    {
                        xw.WriteStartAttribute("stop-opacity");
                        xw.WriteValue((gf.Fills[i].A / 255f).ToString("f3"));
                        xw.WriteEndAttribute();
                    }

                    xw.WriteStartAttribute("offset");
                    xw.WriteFloat(gf.Stops[i] * 100);
                    xw.WriteValue("%");
                    xw.WriteEndAttribute();

                    xw.WriteEndElement(); // stop
                }

                xw.WriteEndElement(); // linearGradient
            }
            else if (gf.FillType == FillType.Radial)
            {
                //<radialGradient id = "g2" cx = "100" cy = "100" r = "50">
                //    <stop stop-color = "green" offset = "0%"/>
                //    <stop stop-color = "pink" offset = "100%"/>
                //</radialGradient>

                xw.WriteStartElement("radialGradient");

                xw.WriteStartAttribute("id");
                xw.WriteValue("gf_" + gf.TagId);
                xw.WriteEndAttribute();
                
                xw.WriteAttributeString("gradientUnits", "userSpaceOnUse");
                xw.WriteAttributeString("cx", "0");
                xw.WriteAttributeString("cy", "0");
                xw.WriteAttributeString("r", GradientFill.GradientVexRect.Right.ToString());

                Matrix m = gf.Transform;
                xw.WriteStartAttribute("gradientTransform");
                xw.WriteValue("matrix(" + m.ScaleX + "," + m.Rotate0 + "," + m.Rotate1 + "," + m.ScaleY + "," + m.TranslateX + "," + m.TranslateY + ")");
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("spreadMethod");
                xw.WriteValue("pad");
                xw.WriteEndAttribute();


                for (int i = gf.Stops.Count - 1; i >= 0 ; i--)
                {
                    xw.WriteStartElement("stop");

                    xw.WriteStartAttribute("stop-color");
                    xw.WriteValue("#" + gf.Fills[i].RGB.ToString("X6"));
                    xw.WriteEndAttribute();

                    if (gf.Fills[i].A < 255)
                    {
                        xw.WriteStartAttribute("stop-opacity");
                        xw.WriteValue((gf.Fills[i].A / 255f).ToString("f3"));
                        xw.WriteEndAttribute();
                    }

                    xw.WriteStartAttribute("offset");
                    xw.WriteFloat((1 - gf.Stops[i]) * 100); // xaml fill is reversed from gdi
                    xw.WriteValue("%");
                    xw.WriteEndAttribute();

                    xw.WriteEndElement(); // stop
                }

                xw.WriteEndElement(); // radialGradient
            }
        }
        private void DefineImage(Image img)
        {
            string brushName = imageBrushPrefix + img.Id.ToString();
            xw.WriteImageBrushRef(brushName, img);
            imageBrushes.Add(img.Path, brushName);
            images.Add(img.Path, img);
        }

        /// <summary>
        /// This ensures nested image refs get defined when displaying only part of file
        /// </summary>
        /// <param name="defs"></param>
        /// <param name="isRoot"></param>
        public void AddImagesPart(Dictionary<uint, IDefinition> defs, bool isRoot)
        {
            uint imgCount = 0;
            foreach (uint key in defs.Keys)
            {
                IDefinition def = defs[key];
                if (defs[key] is Symbol)
                {
                    Symbol sym = (Symbol)defs[key];
                    for (int i = 0; i < sym.Shapes.Count; i++)
                    {
                        Shape sh = sym.Shapes[i];
                        if (sh.Fill is ImageFill)
                        {
                            ImageFill imf = (ImageFill)sh.Fill;
                            Image img = new Image(imf.ImagePath, imgCount++);
                            string brushName = imageBrushPrefix + img.Id.ToString();
                            xw.WriteImageBrushRef(brushName, img);
                            imageBrushes.Add(img.Path, brushName);
                            images.Add(img.Path, img);
                        }
                    }
                }
            }
        }
	
        protected override void RenderPath(FillStyle fs, StrokeStyle ss, List<IShapeData> sh, bool silverlight)
        {
            // <Path Fill="#FFFF0000" 
            // StrokeThickness="0.00491913" StrokeLineJoin="Round" Stroke="#FF014393"
            // Data="M 196.667,4L 388.667,100L 388.667,292L 196.667,388L 4.66669,292L 4.66669,100L 196.667,4 Z "/>
            if (sh.Count == 0)
            {
                return;
            }
            xw.WriteStartElement("path");
            
            if (fs != null)
            {
                switch (fs.FillType)
                {
                    case FillType.Solid:
                        Color c = ((SolidFill)fs).Color;
                        xw.WriteStartAttribute("fill");
                        xw.WriteColor(c);
                        xw.WriteEndAttribute();

                        // try to clean up faint edges
                        if (ss == null && c != new Color(0xFF, 0xFF, 0xFF) && c.A != 0)
                        {
                            ss = new SolidStroke(0.3F, c);
                        }
                        break;

                    case FillType.Linear:
                    case FillType.Radial:
                    case FillType.Focal:
                        GradientFill gf = (GradientFill)fs;
                        // fill="url(#lg)"
                        xw.WriteStartAttribute("fill");
                        xw.WriteValue("url(#gf_" + gf.TagId + ")");
                        xw.WriteEndAttribute();
                        break;

                    case FillType.Image:
                        ImageFill img = (ImageFill)fs;
                        if (img.IsTiled)
                        {
                            //isTiledBitmap = true;
                        }
                        break;
                }
            }
            else
            {
                xw.WriteStartAttribute("fill");
                xw.WriteValue("none");
                xw.WriteEndAttribute();
            }


            if (ss != null)
            {
                if (ss is SolidStroke)
                {
                    // StrokeThickness="3" StrokeLineJoin="Round" Stroke="#FF014393"
                    // StrokeStartLineCap="Round"
                    // StrokeEndLineCap="Round" 
                    SolidStroke st = (SolidStroke)ss;

                    if (st.LineWidth != 0.3f)
                    {
                        xw.WriteStartAttribute("stroke-width");
                        xw.WriteFloat(st.LineWidth);
                        xw.WriteEndAttribute();
                    }
                    
                    xw.WriteStartAttribute("stroke");
                    xw.WriteColor(st.Color);
                    xw.WriteEndAttribute();
                }
            }

            // todo: this is pre defined in svg
            //if (isTiledBitmap)
            //{
            //    //<Path.Fill>
            //    //   <ImageBrush ImageSource="Resources\bmp_1.jpg" TileMode="Tile" RelativeTransform=".2,0,0,.2,0,0"/>
            //    //</Path.Fill>  

            //    ImageFill img = (ImageFill)fs;

            //    xw.WriteStartElement("Path.Fill");
            //    xw.WriteStartElement("ImageBrush");

            //    xw.WriteStartAttribute("ImageSource");
            //    xw.WriteValue(img.ImagePath);
            //    xw.WriteEndAttribute();

            //    if (!silverlight)
            //    {
            //        xw.WriteStartAttribute("TileMode");
            //        xw.WriteValue("Tile");
            //        xw.WriteEndAttribute();
            //    }

            //    Matrix pMatrix = ApplyMatrixToShape(sh, img.Matrix, images[img.ImagePath].StrokeBounds);
            //    //Matrix pMatrix = ApplyMatrixToImage(img.Matrix, images[img.ImagePath].Bounds);
            //    xw.WriteStartAttribute("RelativeTransform");
            //    xw.WriteMatrix(pMatrix);
            //    //xw.WriteMatrix(img.Matrix);
            //    xw.WriteEndAttribute();

            //    xw.WriteEndElement(); // Path.Fill
            //    xw.WriteEndElement(); // ImageBrush
            //}

            
            xw.WriteStartAttribute("d");
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
                        break;

                    case SegmentType.QuadraticBezier:
                        QuadBezier qb = (QuadBezier)sd;
                        xw.WriteQuadraticCurveTo(qb.Control, qb.Anchor1);
                        lastPoint = qb.EndPoint;
                        break;
                }

            }
            xw.WriteEndAttribute();
            xw.WriteEndElement(); // Path
        }

        protected override void WriteTransformDef(Instance inst, string fullName)
        {
            //<g transform="matrix(a, b, c, d, e, f)">
            //    <use xlink:href="#example" />
            //</g> 
            
            Matrix m = inst.Transformations[0].Matrix;

            bool hasTranslate = (m.Location != Point.Zero);
            bool hasAffine = (m.ScaleX != 1) || (m.Rotate0 != 0) || (m.Rotate1 != 0) || (m.ScaleY != 1);

            if (hasTranslate || hasAffine)
            {
                xw.WriteStartAttribute("transform");

                string s = "matrix(";
                s += m.ScaleX.ToString("0.###") +",";
                s += m.Rotate0.ToString("0.###") + ",";
                s += m.Rotate1.ToString("0.###") + ",";
                s += m.ScaleY.ToString("0.###") + ",";
                s += m.Location.X.ToString("0.###") + ",";
                s += m.Location.Y.ToString("0.###") + ")";

                xw.WriteValue(s);
                xw.WriteEndAttribute();
            }
        }
    }
}
