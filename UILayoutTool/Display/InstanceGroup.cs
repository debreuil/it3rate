using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using System.Drawing;
using SysMatrix = System.Drawing.Drawing2D.Matrix;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DDW.Managers;
using DDW.Utils;
using DDW.Assets;
using DDW.Enums;
using DDW.Views;
using DDW.Vex.Bonds;

namespace DDW.Display
{
    public class InstanceGroup : IDataObject
    {
        public static string formatName = "InstanceGroup";
        private InstanceManager instanceManager;

        public event EventHandler ContentsChanged;

        private List<uint> list = new List<uint>();
        
        private uint[] relatedIds = new uint[] { };
        public uint[] RelatedIds { get { return relatedIds; } }        private bool relatedAreInvalid;
        public bool RelatedAreInvalid { get { return relatedAreInvalid; } set { relatedAreInvalid = value; } }


        private Vex.Point location;
        private Vex.Rectangle strokeBounds;
        private Vex.Point center;
        private bool isDirty;
        private bool isTransformDirty = true;
        private Vex.Rectangle untransformedBounds;
        public Vex.Rectangle UntransformedBounds
        {
            get
            {
                if (isTransformDirty)
                {
                    ResetTransform();
                }

                return untransformedBounds;
            }
        }
        private Matrix transformMatrix;
        public Matrix TransformMatrix
        {
            get
            {
                if (isTransformDirty)
                {
                    ResetTransform();
                }
                return transformMatrix;
            }
            set
            {
                if (transformMatrix != null)
                {
                    transformMatrix.Dispose();
                }
                transformMatrix = value;
            }
        }
        public Vex.Matrix TransformMatrixStage
        {
            get
            {
                Matrix sm = MainForm.CurrentStage.CameraMatrix.Clone();
                sm.Multiply(TransformMatrix, MatrixOrder.Append);
                Vex.Matrix result = sm.VexMatrix();
                sm.Dispose();

                return result;
            }
        }
        public void ResetTransform()
        {
            if (transformMatrix != null)
            {
                transformMatrix.Dispose();
            }

            if (list.Count == 1)
            {
                DesignInstance di = instanceManager[list[0]];
                transformMatrix = di.GetMatrix().SysMatrix();
                center = di.RotationCenter;
                untransformedBounds = di.Definition.StrokeBounds;//.TranslatedRectangle(-di.Location.X, -di.Location.Y);
            }
            else
            {
                transformMatrix = new Matrix();
                transformMatrix.Translate(StrokeBounds.Left, StrokeBounds.Top, MatrixOrder.Append);
                center = StrokeBounds.LocalCenter;
                //center = StrokeBounds.LocalCenter.Translate(StrokeBounds.Point);
                untransformedBounds = StrokeBounds.TranslatedRectangle(-StrokeBounds.Left, -StrokeBounds.Top);
            }

            isTransformDirty = false;
        }

        public Vex.Point GlobalRotationCenter
        {
            get
            {
                return LocalToTransform(center);
            }
            set
            {
                center = TransformToLocal(value);
                if (list.Count == 1)
                {
                    instanceManager[list[0]].RotationCenter = center;
                }
            }
        }
        public Vex.Point SelectionCenter
        {
            get
            {
                return LocalToTransform(StrokeBounds.LocalCenter);
            }
        }

        public Vex.Point LocalToTransform(Vex.Point p)
        {
            return Gdi.GdiRenderer.GetTransformedPoint(p.SysPointF(), TransformMatrix, false).VexPoint();
        }
        public Vex.Point TransformToLocal(Vex.Point p)
        {
            return Gdi.GdiRenderer.GetTransformedPoint(p.SysPointF(), TransformMatrix, true).VexPoint();
        }

        public InstanceGroup(InstanceManager instanceManager)
        {
            this.instanceManager = instanceManager;
        }

        public uint this[int index]
        {
            get { return list[index]; }
        }
        public Vex.Rectangle StrokeBounds
        {
            get
            {
                if (isDirty)
                {
                    CalculateBounds();
                    isDirty = false;
                }
                return strokeBounds;
            }
        }
        public Vex.Point Location
        {
            get
            {
                if (isDirty)
                {
                    CalculateBounds();
                    isDirty = false;
                } 
                return location;
            }
        }
        public bool IsSingleRotated
        {
            get
            {
                bool result = false;
                if (list.Count == 1)
                {
                    Vex.Matrix m = instanceManager[list[0]].GetMatrix();
                    result =  m.Rotate0 != 0 || m.Rotate1 != 0;
                }
                return result;
            }
        }
        public int Count { get { return list.Count; } }
        public uint[] SelectedIds { get { return list.ToArray(); } }

        public PointF[] GetTransformedPoints(Matrix globalMatrix)
        {
            Vex.Rectangle tb = UntransformedBounds;
            PointF[] pts = tb.SysPointFs();
            using (Matrix m = TransformMatrix.Clone())
            {
                m.Multiply(globalMatrix, MatrixOrder.Append);
                m.TransformPoints(pts);
            }
            return pts;
        }
        public PointF[] GetTransformedPoints()
        {
            Vex.Rectangle tb = UntransformedBounds;//.TranslatedRectangle(StrokeBounds.Left, StrokeBounds.Top);
            PointF[] pts = tb.SysPointFs();
            TransformMatrix.TransformPoints(pts);
            return pts;
        }
        public PointF[] GetTransformedCenter()
        {
            PointF[] pts = new PointF[] { UntransformedBounds.Center.SysPointF() };
            TransformMatrix.TransformPoints(pts);
            return pts;
        }
                
        public void Update()
        {
            isDirty = true;
            isTransformDirty = true;
            relatedAreInvalid = true;
        }
        public bool IsEmpty { get { return list.Count == 0; } }

        public void Set(uint[] ids)
        {
            if (!ArrayEquals(ids, list.ToArray()))
            {
                list.Clear();
                strokeBounds = Vex.Rectangle.Empty;
                foreach (uint id in ids)
                {
                    list.Add(id);
                }

                isDirty = true;
                isTransformDirty = true;
                relatedAreInvalid = true;
                OnContentsChanged();
            }
        }
        public void Add(uint inst)
        {
            list.Add(inst);
            isDirty = true;
            isTransformDirty = true;
            OnContentsChanged();
        }
        public void AddRange(IEnumerable<uint> collection)
        {
            list.AddRange(collection);
            isDirty = true;
            isTransformDirty = true;
            OnContentsChanged();
        }
        public void Remove(uint inst)
        {
            list.Remove(inst);
            isDirty = true;
            isTransformDirty = true;
            OnContentsChanged();
        }
        public void RemoveRange(int index, int count)
        {
            list.RemoveRange(index, count);
            isDirty = true;
            OnContentsChanged();
        }
        public bool Contains(uint inst)
        {
            return list.Contains(inst);
        }
        public void Clear()
        {
            list.Clear();
            strokeBounds = Vex.Rectangle.Empty;
            isDirty = true;
            isTransformDirty = true;
            relatedAreInvalid = true;
            OnContentsChanged();
        }

        private void OnContentsChanged()
        {
            HashSet<uint> alreadyDiscovered = new HashSet<uint>();
            MainForm.CurrentStage.CurrentEditItem.BondStore.GetRelatedObjects(list, alreadyDiscovered);
            relatedIds = alreadyDiscovered.ToArray();

            if (ContentsChanged != null)
            {
                ContentsChanged(this, EventArgs.Empty);
            }
        }

        public bool IsMatchingSet(uint[] indexes)
        {
            bool result = false;
            if (list.Count == indexes.Length)
            {
                uint[] curItems = list.ToArray();
                Array.Sort(indexes);
                Array.Sort(curItems);
                return ArrayEquals(indexes, curItems);
            }
            return result;
        }
        public uint[] IdsByDepth
        {
            get
            {
                SortedList<int, uint> sl = new SortedList<int, uint>();
                for (int i = 0; i < list.Count; i++)
                {
                    DesignInstance di = instanceManager[list[i]];
                    sl.Add(di.Depth, list[i]);
                }
                return sl.Values.ToArray();
            }
        }   
        
        public Bitmap GetOutline()
        {
            Bitmap bmp = null;
            if (StrokeBounds != Vex.Rectangle.Empty)
            {
                RectangleF sbt = MainForm.CurrentStage.StageToCamera(StrokeBounds.SysRectangleF());
                bmp = Gdi.GdiRenderer.GetEmptyBitmap(sbt.Size);
                Graphics g = Gdi.GdiRenderer.GetGraphicsFromBitmap(bmp);
                g.Transform = MainForm.CurrentStage.CameraMatrix;
                DrawOutlineInto(g, -sbt.Left, -sbt.Top);
                g.Dispose();
            }
            return bmp;
        }

        public void ScaleAt(float scaleX, float scaleY, Vex.Point center)
        {
            scaleX = (Math.Abs(scaleX) < .01) ? .01f : scaleX;
            scaleY = (Math.Abs(scaleY) < .01) ? .01f : scaleY;

            Matrix tm = TransformMatrix.Clone();
            tm.Translate(-TransformMatrix.OffsetX, -TransformMatrix.OffsetY, MatrixOrder.Append);
            Matrix itm = tm.Clone();
            itm.Invert();

            foreach (uint id in list)
            {
                DesignInstance inst = instanceManager[id];

                using (Matrix ms = inst.GetMatrix().SysMatrix())
                {
                    ms.Translate(-center.X, -center.Y, MatrixOrder.Append);
                    ms.Multiply(itm, MatrixOrder.Append);
                    ms.Scale(scaleX, scaleY, MatrixOrder.Append);
                    ms.Multiply(tm, MatrixOrder.Append);
                    ms.Translate(center.X, center.Y, MatrixOrder.Append);

                    inst.SetMatrix(ms.VexMatrix());
                }
            }
            tm.Dispose();
            itm.Dispose();

            transformMatrix.ScaleAt(scaleX, scaleY, center.SysPointF());
            isDirty = true;
            MainForm.CurrentStage.InvalidateTransformedSelection();
        }
        public void DrawScaled(Graphics g, float scaleX, float scaleY, Vex.Point center)
        {
            Matrix m = g.Transform;
            scaleX = (Math.Abs(scaleX) < .01) ? .01f : scaleX;
            scaleY = (Math.Abs(scaleY) < .01) ? .01f : scaleY;

            Matrix tm = TransformMatrix.Clone();
            tm.Translate(-TransformMatrix.OffsetX, -TransformMatrix.OffsetY, MatrixOrder.Append);
            Matrix itm = tm.Clone();
            itm.Invert();

            foreach (uint id in list)
            {
                DesignInstance inst = instanceManager[id];

                using (Matrix ms = inst.GetMatrix().SysMatrix())
                {
                    ms.Translate(-center.X, -center.Y, MatrixOrder.Append);
                    ms.Multiply(itm, MatrixOrder.Append);
                    ms.Scale(scaleX, scaleY, MatrixOrder.Append);
                    ms.Multiply(tm, MatrixOrder.Append);
                    ms.Translate(center.X, center.Y, MatrixOrder.Append);

                    ms.Multiply(m, MatrixOrder.Append);
                    g.Transform = ms;
                    inst.DrawOutlineIntoRaw(g, 0, 0);
                }
            }

            tm.Dispose();
            itm.Dispose();

            g.Transform = m;
        }
        public Matrix GetScaledMatrix(float scaleX, float scaleY, Vex.Point center)
        {
            Matrix m = transformMatrix.Clone();
            m.ScaleAt(scaleX, scaleY, center.SysPointF());
            return m;
        }

        public void RotateAt(float angle, Vex.Point center)
        {
            foreach (uint id in list)
            {
                DesignInstance inst = instanceManager[id];
                using (Matrix mr = inst.GetMatrix().SysMatrix())
                {
                    mr.RotateAt(angle, center.SysPointF(), MatrixOrder.Append);
                    inst.SetMatrix(mr.VexMatrix());
                }
            }

            transformMatrix.RotateAt(angle, center.SysPointF(), MatrixOrder.Append);
            isDirty = true;
            MainForm.CurrentStage.InvalidateTransformedSelection();
        }        
        public void DrawRotated(Graphics g, float angle, Vex.Point center)
        {
            Matrix m = g.Transform;
            foreach (uint id in list)
            {
                DesignInstance inst = instanceManager[id];
                using (Matrix mr = inst.GetMatrix().SysMatrix())
                {
                    mr.RotateAt(angle, center.SysPointF(), MatrixOrder.Append);
                    mr.Multiply(m, MatrixOrder.Append);
                    g.Transform = mr;
                    inst.DrawOutlineIntoRaw(g, 0, 0);
                }
            }
            g.Transform = m;
        }
        public Matrix GetRotatedMatrix(float angle, Vex.Point center)
        {
            Matrix m = transformMatrix.Clone();
            m.RotateAt(angle, center.SysPointF(), MatrixOrder.Append);
            return m;
        }

        public void TranslateSelection(Vex.Point offset)
        {
            Dictionary<uint, Vex.Rectangle> alreadyDiscovered = new Dictionary<uint, Vex.Rectangle>();
            MainForm.CurrentStage.CurrentEditItem.BondStore.GetRelatedObjectTransforms(list, alreadyDiscovered, offset);
            relatedIds = alreadyDiscovered.Keys.ToArray();
            foreach (uint id in alreadyDiscovered.Keys)
            {
                TranslateElement(id, alreadyDiscovered[id].Left, alreadyDiscovered[id].Top);
            }

            ResetTransform();
            MainForm.CurrentStage.InvalidateTransformedSelection();
        }
        private void TranslateElement(uint id, float newBoundsLeft, float newBoundsTop)
        {
            DesignInstance inst = instanceManager[id];
            using (Matrix mr = inst.GetMatrix().SysMatrix())
            {
                mr.Translate(newBoundsLeft - inst.StrokeBounds.Left, newBoundsTop - inst.StrokeBounds.Top, MatrixOrder.Append);
                inst.SetMatrix(mr.VexMatrix());
            }

            isDirty = true;
        }
        public void DrawTranslated(Graphics g, uint id, float newLocX, float newLocY)
        {
            Matrix m = g.Transform;

            DesignInstance inst = instanceManager[id];
            using (Matrix mr = inst.GetMatrix().SysMatrix())
            {
                mr.Translate(newLocX - inst.Location.X, newLocY - inst.Location.Y, MatrixOrder.Append);
                mr.Multiply(m, MatrixOrder.Append);
                g.Transform = mr;
                inst.DrawOutlineIntoRaw(g, 0, 0);
            }

            g.Transform = m;
        }
        public Matrix GetTranslatedMatrix(float offsetX, float offsetY)
        {
            Matrix m = transformMatrix.Clone();
            m.Translate(offsetX, offsetY, MatrixOrder.Append);
            return m;
        }

        public void DrawInto(Graphics g, float offsetX, float offsetY)
        {
            foreach (uint id in list)
            {
                DesignInstance inst = instanceManager[id];
                inst.DrawInto(g, offsetX, offsetY);
            }
        }
        public void DrawOutlineInto(Graphics g, float offsetX, float offsetY)
        {
            foreach (uint id in list)
            {
                DesignInstance inst = instanceManager[id];
                inst.DrawOutlineInto(g, offsetX, offsetY);
            }
        }

        private void CalculateBounds()
        {
            strokeBounds = CalculateBounds(list.ToArray(), out location);
            MainForm.CurrentStage.OnEdit();
        }

        public Vex.Rectangle CalculateBounds(uint[] instances, out Vex.Point topLeft)
        {
            Vex.Rectangle bounds = Vex.Rectangle.Empty;
            float top = int.MaxValue;
            float left = int.MaxValue;
            foreach (uint id in instances)
            {
                DesignInstance inst = instanceManager[id];
                if (bounds.IsEmpty)
                {
                    bounds = inst.StrokeBounds;
                }
                else
                {
                    bounds = inst.StrokeBounds.Union(bounds);// Rectangle.Union(bounds, inst.Bounds);
                }

                left = Math.Min(left, inst.Location.X);
                top = Math.Min(top, inst.Location.Y);
            }
            topLeft = new Vex.Point(left, top);
            return bounds;
        }

        private bool ArrayEquals(uint[] ar0, uint[] ar1)
        {
            bool result = false;
            if (ar0.Length == ar1.Length)
            {
                for (int i = 0; i < ar0.Length; i++)
                {
                    if (ar0[i] != ar1[i])
                    {
                        result = false;
                        break;
                    }
                    result = true;
                }
            }
            return result;
        }
        
        #region IDataObject
        public object GetData(Type format)
        {
            return this;
        }

        public object GetData(string format)
        {
            return this;
        }

        public object GetData(string format, bool autoConvert)
        {
            return this;
        }

        public bool GetDataPresent(Type format)
        {
            return format == this.GetType();
        }

        public bool GetDataPresent(string format)
        {
            return format == formatName;
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            return format == formatName;
        }

        public string[] GetFormats()
        {
            return new string[] { formatName };
        }

        public string[] GetFormats(bool autoConvert)
        {
            return new string[] { formatName };
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

    }
}
