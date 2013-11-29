using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using SysMatrix = System.Drawing.Drawing2D.Matrix;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DDW.Display;
using System.Drawing.Imaging;
using DDW.Assets;
using DDW.Interfaces;
using DDW.Managers;
using DDW.Utils;
using Vex = DDW.Vex;
using System.Drawing.Drawing2D;
using DDW.Views;
using System.IO;
using System.Xml.Serialization;

namespace DDW.Display
{
    public partial class DesignInstance : IDrawable, IDataObject
    {
        #region Feilds and Properties
        public static string formatName = "SymbolInstance";
        protected Vex.IInstance instance;
        protected StageView stage;

        public bool HasSaveableChanges { get { return instance.HasSaveableChanges; } set { instance.HasSaveableChanges = value; } }

        public uint ParentInstanceId { get; set; }
        
        public Vex.IInstance Instance { get { return instance; } }
        public Vex.Timeline ParentDefinition
        {
            get
            {
                Vex.Timeline result = null;
                if (stage.Library.Contains(instance.ParentDefinitionId))
                {
                    result = (Vex.Timeline)stage.Library[instance.ParentDefinitionId].Definition;
                }
                return result;
            }
        }
        public bool IsSelected
        {
            get
            {
                bool result = false;
                if (stage.Selection.Contains(this.InstanceHash))
                {
                    result = true;
                }
                return result;
            }
        }

        public uint DefinitionId { get { return instance.DefinitionId; } }
        public LibraryItem LibraryItem { get { return stage.Library[DefinitionId]; } }
        public Vex.IDefinition Definition { get { return stage.Library[DefinitionId].Definition; } }
        public uint InstanceHash { get { return instance.InstanceHash; } set { instance.InstanceHash = value; HasSaveableChanges = true;} }
        public string InstanceName { get { return instance.Name; } set { instance.Name = value; HasSaveableChanges = true; } }

        public Vex.Point Location
        {
            get
            {
                return GetMatrix().Location;
            }
        }
        public Vex.Point RotationCenter
        {
            get { return instance.RotationCenter; }
            set { instance.RotationCenter = value; HasSaveableChanges = true; }
        }
        public bool IsRotated
        {
            get
            {
                Vex.Matrix m = GetMatrix();
                return m.Rotate0 != 0 || m.Rotate1 != 0;
            }
        }
        public Vex.Rectangle StrokeBounds
        {
            get
            {
                return Gdi.GdiRenderer.GetTransformedBounds(Definition, GetSysMatrix(), false).VexRectangle();
            }
        }
        public Vex.Rectangle UntransformedBounds
        {
            get
            {
                return LibraryItem.Definition.StrokeBounds;
            }
        }
        public int Depth
        {
            get
            {
                int result = -1;
                if (ParentDefinition != null)
                {
                    result = ParentDefinition.GetInstanceDepth(InstanceHash);
                }
                return result;
            }
        }
        public Vex.AspectConstraint AspectConstraint
        {
            get
            {
                Vex.AspectConstraint result = Vex.AspectConstraint.None;
                if (instance is Vex.Instance)
                {
                    result = ((Vex.Instance)instance).AspectConstraint;
                }
                return result;
            }
            set
            {
                if (instance is Vex.Instance)
                {
                    ((Vex.Instance)instance).AspectConstraint = value;
                }
            }
        }

        protected Bitmap cachedBitmap;

        private static ImageAttributes ia;

        private Pen selfConstraintPen = new Pen(Color.Red, 2);
        private Pen wideWhitePen = new Pen(Color.White, 2);

        #endregion

        #region ctor/Init
        public DesignInstance(StageView stage, Vex.Instance inst)
        {
            this.stage = stage;
            instance = inst;
            Initialize(); // will not set loc if there are already transforms
        }
        
        private void Initialize()
        {
            RotationCenter = LibraryItem.Definition.StrokeBounds.Center;

            if (ia == null)
            {
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.5f;
                ia = new ImageAttributes();
                ia.SetColorMatrix(cm);
            }

            selfConstraintPen.DashPattern = new float[] { 2f, 3f };

            HasSaveableChanges = true;
        }
        #endregion


        public void GetFullMatrix(out Matrix m)
        {
            if (stage.InstanceManager.Contains(ParentInstanceId))
            {
                DesignInstance parent = stage.InstanceManager[ParentInstanceId];
                m = parent.GetMatrix().SysMatrix();
                m.Invert();
            }
            else
            {
                m = new SysMatrix();
            }

            using (Matrix lm = instance.GetTransformAtTime(0).Matrix.SysMatrix())
            {
                lm.Invert();
                m.Multiply(lm, MatrixOrder.Append);
            }
        }
        private SysMatrix sysMatrix;
        public SysMatrix GetSysMatrix()
        {
            if (sysMatrix == null)
            {
                sysMatrix = GetMatrix().SysMatrix();
            }
            return sysMatrix;
        }
        public Vex.Matrix GetMatrix()
        {
            Vex.Transform t = instance.GetTransformAtTime(0);
            return t.Matrix;
        }
        public void SetMatrix(Vex.Matrix m)
        {
            Vex.Transform t = instance.GetTransformAtTime(0);
            t.Matrix = m;
            if (sysMatrix != null)
            {
                sysMatrix.Dispose();
            }
            sysMatrix = m.SysMatrix();
            HasSaveableChanges = true;
        }


        public PointF[] GetTransformedPoints()
        {
            Vex.Rectangle tb = UntransformedBounds;
            PointF[] pts = tb.SysPointFs();
            using (Matrix m = GetSysMatrix().Clone())
            {
                m.TransformPoints(pts);
            }
            return pts;
        }
        public PointF[] GetTransformedCenter()
        {
            PointF[] pts = new PointF[]{UntransformedBounds.Center.SysPointF()};
            using (Matrix m = GetSysMatrix().Clone())
            {
                m.TransformPoints(pts);
            }
            return pts;
        }
        public PointF[] GetTransformedPoints(Matrix globalMatrix)
        {
            Vex.Rectangle tb = UntransformedBounds;
            PointF[] pts = tb.SysPointFs();
            using (Matrix m = GetSysMatrix().Clone())
            {
                m.Multiply(globalMatrix, MatrixOrder.Append);
                m.TransformPoints(pts);
            }
            return pts;
        }
        public Vex.Point ContainerToLocal(Vex.Point pt)
        {
            Vex.Point result = Gdi.GdiRenderer.GetTransformedPoint(pt.SysPointF(), GetSysMatrix(), true).VexPoint();
            return result;
        }
        public Vex.Point LocalToContainer(Vex.Point pt)
        {
            Vex.Point result = Gdi.GdiRenderer.GetTransformedPoint(pt.SysPointF(), GetSysMatrix(), false).VexPoint();
            return result;
        }

        public PointF ContainerToLocal(PointF pt)
        {
            PointF result = Gdi.GdiRenderer.GetTransformedPoint(pt, GetSysMatrix(), true);
            return result;
        }
        public PointF LocalToContainer(PointF pt)
        {
            PointF result = Gdi.GdiRenderer.GetTransformedPoint(pt, GetSysMatrix(), false);
            return result;
        }

        #region Paint
        public virtual void Draw(Graphics g)
        {
            //SysMatrix orgM = g.Transform;
            //SysMatrix newM = GetMatrix().SysMatrix();
            //newM.Multiply(orgM, MatrixOrder.Append);
            //g.Transform = newM;

            stage.Gdi.RenderInto(Definition, 0, g);

            //newM.Dispose();
            //g.Transform = orgM;
        }
        public void DrawMaskInto(Graphics g, Color c)
        {
            SysMatrix orgM = g.Transform;
            SysMatrix newM = GetMatrix().SysMatrix();
            newM.Multiply(orgM, MatrixOrder.Append);
            g.Transform = newM;

            stage.Gdi.RenderMaskInto(Definition, 0, g, c);

            newM.Dispose();
            g.Transform = orgM;
        }

        public void DrawOutlineIntoRaw(Graphics g, float offsetX, float offsetY)
        {
            stage.Gdi.RenderOutlineInto(Definition, 0, g);
        }
        public void DrawOutlineInto(Graphics g, float offsetX, float offsetY)
        {
            SysMatrix orgM = g.Transform;

            SysMatrix newM = GetSysMatrix().Clone();
            newM.Multiply(orgM, MatrixOrder.Append);
            newM.Translate(offsetX, offsetY, MatrixOrder.Append);
            g.Transform = newM;
            stage.Gdi.RenderOutlineInto(Definition, 0, g);
            newM.Dispose();

            g.Transform = orgM;
        }
        public void DrawInto(Graphics g, float offsetX, float offsetY)
        {
            SysMatrix orgM = g.Transform;

            SysMatrix newM = GetMatrix().SysMatrix();
            newM.Multiply(orgM, MatrixOrder.Append);
            newM.Translate(-offsetX, -offsetY, MatrixOrder.Append);
            g.Transform = newM;
            stage.Gdi.RenderInto(Definition, 0, g);
            newM.Dispose();

            g.Transform = orgM;
        }

        public void DrawAspectConstraints(Graphics g, Vex.AspectConstraint ac, PointF[] pts)
        {
            if (ac != Vex.AspectConstraint.None)
            {
                switch (ac)
                {
                    case Vex.AspectConstraint.Horizontal:
                        g.DrawLine(wideWhitePen, pts[7], pts[3]);
                        g.DrawLine(selfConstraintPen, pts[7], pts[3]);
                        break;
                    case Vex.AspectConstraint.Vertical:
                        g.DrawLine(wideWhitePen, pts[1], pts[5]);
                        g.DrawLine(selfConstraintPen, pts[1], pts[5]);
                        break;
                    case Vex.AspectConstraint.Locked:
                        g.DrawLine(wideWhitePen, pts[0], pts[4]);
                        g.DrawLine(selfConstraintPen, pts[0], pts[4]);
                        break;
                }
            }
        }

        Brush debugBkg = new SolidBrush(Color.FromArgb(0xA0, Color.White));
        public void DrawDebugInto(Graphics g, PointF location)
        {
            Matrix orgM = g.Transform;
            Matrix newM = orgM.Clone();
            newM.Translate(location.X, location.Y, MatrixOrder.Append);
            g.Transform = newM;

            Vex.Matrix m = GetMatrix();
            string text = "i: " + instance.InstanceHash + " d: " + instance.DefinitionId +" dp: " + instance.Depth + "\n";
            text += "loc: " + Location.X.ToString("F2") + " : " + Location.Y.ToString("F2") + "\n";
            text += "mx: " + 
                m.ScaleX.ToString("F2") + ", " + 
                m.Rotate0.ToString("F2") + ", " +
                m.Rotate1.ToString("F2") + ", " + 
                m.ScaleY.ToString("F2") + "\n";

            SizeF sz = g.MeasureString(text, SystemFonts.DefaultFont);

            Point loc = new Point((int)(-sz.Width / 2), (int)(-sz.Height / 2));
            g.FillRectangle(debugBkg, new Rectangle(loc, Size.Ceiling(sz)));
            g.DrawString(text, SystemFonts.DefaultFont, Brushes.DarkRed, loc);

            g.Transform = orgM;
            newM.Dispose();
        }
        #endregion

        #region IDataObject
        public object GetData(Type format)
        {
            object result = null;

            if (format == this.GetType())
            {
                result = this;
            }
            else if (format == LibraryItem.GetType())
            {
                result = LibraryItem;
            }

            return result;
        }

        public object GetData(string format)
        {
            object result = null;

            if (format == formatName)
            {
                result = this;
            }
            else if (format == DDW.Display.LibraryItem.formatName)
            {
                result = LibraryItem;
            }

            return result;
        }

        public object GetData(string format, bool autoConvert)
        {
            return GetData(format);
        }

        public bool GetDataPresent(Type format)
        {
            return format == LibraryItem.GetType();
        }

        public bool GetDataPresent(string format)
        {
            return format == formatName || format == DDW.Display.LibraryItem.formatName;
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            return format == formatName || format == DDW.Display.LibraryItem.formatName;
        }

        public string[] GetFormats()
        {
            return new string[] { formatName, DDW.Display.LibraryItem.formatName };
        }

        public string[] GetFormats(bool autoConvert)
        {
            return new string[] { formatName, DDW.Display.LibraryItem.formatName };
        }

        public void SetData(object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(Type format, object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(string format, object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(string format, bool autoConvert, object data)
        {
            throw new NotImplementedException();
        }

        #endregion


        public string GetDataPath()
        {
            string result = "inst_" + InstanceHash + ".xml";
            return result;
        }

        public bool Save(string folderPath)
        {
            bool result = false;
            // type (timeline or instance) // or later 9box, array..
            // InstanceId
            // DefinitionId
            // ParentInstanceId
            // Instance
            // RotationCenter

            instance.Depth = this.Depth;

            FileStream fs;

            string dataFileName = folderPath + System.IO.Path.DirectorySeparatorChar + GetDataPath();

            //save definition
            File.WriteAllText(dataFileName, string.Empty);
            fs = new FileStream(dataFileName, FileMode.OpenOrCreate);

            XmlSerializer xs = new XmlSerializer(typeof(Vex.Instance));
            xs.Serialize(fs, this.Instance);
            fs.Close();

            HasSaveableChanges = false;

            return result;
        }

        public static DesignInstance LoadFromPath(StageView stage, string type, string dataPath)
        {
            DesignInstance result = null;

            FileStream fs = new FileStream(dataPath, FileMode.Open);
            XmlSerializer xs = new XmlSerializer(typeof(Vex.Instance));

            Vex.Instance inst = (Vex.Instance)xs.Deserialize(fs);
            fs.Close();

            result = stage.CreateInstance(inst);
            if (result != null)
            {
                result.HasSaveableChanges = false;
            }
            return result;
        }
    }

}
