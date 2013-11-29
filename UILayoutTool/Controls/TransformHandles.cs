using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DDW.Interfaces;
using Vex = DDW.Vex;
using DDW.Assets;
using DDW.Enums;
using DDW.Utils;
using DDW.Views;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using DDW.Vex.Bonds;
using DDW.Controls;

namespace DDW.Display
{
    public class TransformHandles : IDrawable
    {
        private StageView stage;
        private TransformKind transformKind = TransformKind.Scale;
        public TransformKind TransformKind { get { return transformKind; } }
        
        private static int handleSize = 10;
        private static int maskHandleRadius = 8;

        private Brush boxColor = new SolidBrush(Color.DarkGray);
        private Pen selectionRectPen = new Pen(Color.DarkGray, 0);
        private Size hs;
        private Size hsSmall;

        public BondAttachment AspectConstrainTarget = BondAttachment.None;
        private Pen selfConstraintTargetPen = new Pen(Color.LimeGreen, 2);

        public TransformHandles(StageView stage)
        {
            this.stage = stage;
            hs = new Size(handleSize, handleSize);
            hsSmall = new Size(handleSize - 2, handleSize - 2);
        }

        public void Reset()
        {
            transformKind = TransformKind.Scale;
        }

        public void SetAsRotate()
        {
            transformKind = TransformKind.Rotate;
        }

        public TransformKind ToggleKind()
        {
            transformKind = (transformKind == TransformKind.Scale) ? TransformKind.Rotate : TransformKind.Scale;
            //transformKind = ((int)transformKind + 0x10 > 0x60) ? TransformKind.Scale : (TransformKind)((int)(transformKind + 0x10));
            return transformKind;
        }

        public static Rectangle GetInvalidBounds(PointF[] points)
        {
            Rectangle result = Rectangle.Empty;
            if (points.Length >= 4)
            {
                Rectangle r = points.GetBounds();
                r.Inflate(handleSize + 2, handleSize + 2);
                result = r;
            }
            return result;
        }
        public static Rectangle GetCenterBoundsFromPoint(PointF cp)
        {
            return new Rectangle(
                (int)(cp.X - maskHandleRadius - 1),
                (int)(cp.Y - maskHandleRadius - 1),
                (int)(maskHandleRadius * 2 + 2),
                (int)(maskHandleRadius * 2 + 2));
        }

        private int[] segTableA = new int[] { 1, 1, -1, -1, -1, -1, 1, 1 };
        private int[] segTableB = new int[] { 1, 1, 1, 1, -1, -1, -1, -1 };
        public void Draw(Graphics g)
        {
            PointF[] points = stage.TransformPointsCamera;

            if (points != null && points.Length == 4) // selection may be null
            {
                PointF[] pts;
                pts = points.GetMidpointsAndCenter();
                pts[8] = pts[0]; // use wrap instead of center

                int handleRadius = (int)(handleSize / 2f);
                float rad = handleSize / 2f;

                float angle = (float)Math.Atan2(points[1].Y - points[0].Y, points[1].X - points[0].X);
                float xDif = (float)Math.Cos(angle) * rad;
                float yDif = (float)Math.Sin(angle) * rad;

                DrawOutlineBox(g, xDif * 2f, yDif * 2f, pts);

                DrawSelectionHandles(g, pts);
            }

        }
        
        private void DrawSelectionHandles(Graphics g, PointF[] pts)
        {
            BondType[] bondTypes = (stage.Selection.Count == 1) ?
                stage.CurrentEditItem.BondStore.GetHandlesForInstance(stage.Selection[0]) :
                BondStore.emptyHandles;

            float hr = handleSize / 2f;
            for (int i = 0; i < pts.Length - 1; i++)
            {
                HandleIcon handleIcon = GetHandleIcon(bondTypes[i], transformKind);
                // draw bond icons on separate pass
                if (handleIcon == HandleIcon.Handle || handleIcon == HandleIcon.Rotate)
                {
                    int variationIndex = i % icons[(int)handleIcon].Length;
                    Point loc = new Point((int)(pts[i].X - hr), (int)(pts[i].Y - hr));
                    g.DrawImage(icons[(int)handleIcon][variationIndex], loc);
                    if (AspectConstrainTarget != BondAttachment.None && AspectConstrainTarget.GetHandleIndex() == i)
                    {
                        g.DrawRectangle(selfConstraintTargetPen, new Rectangle(loc.X, loc.Y, handleSquare.Width, handleSquare.Height));
                    }
                }
            }

            if (transformKind == Enums.TransformKind.Rotate)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.Default;

                // center dot
                if (transformKind == TransformKind.Rotate)
                {
                    PointF center = stage.RotationCenterToCamera;
                    Rectangle centerRect = new Rectangle(
                        (int)(center.X - hr),
                        (int)(center.Y - hr),
                        (int)(hr * 2),
                        (int)(hr * 2));

                    g.FillEllipse(Brushes.White, centerRect);
                    g.DrawEllipse(Pens.Black, centerRect);
                }
            }
        }

        private List<Bond> drawnBonds = new List<Bond>();
        private Dictionary<long, PointF> bondPoints = new Dictionary<long, PointF>();
        private HashSet<BondLine> bondLines = new HashSet<BondLine>();

        // todo: this sequences is essentially the same as InstanceGroup>TranslateRelated. Integrate?
        public void DrawBondHandles(DesignTimeline parent, PointF[] points, uint instanceId)
        {
            List<Bond> bonds = new List<Bond>();
            parent.BondStore.GetBondsForInstance(instanceId, bonds);

            for (int i = 0; i < bonds.Count; i++)
            {
                Bond bond = bonds[i];
                drawnBonds.Add(bond);

                if (bond.ChainType.IsDistributed()) // dist
                {
                    if (bond.Previous != null)
                    {
                        int handleIndex = bond.ChainType.GetAttachment().GetHandleIndex();
                        DrawBond(bond, points[handleIndex], handleIndex);
                    }
                    if (bond.Next != null)
                    {
                        int oppHandleIndex = bond.ChainType.GetOppositeAttachment().GetHandleIndex();
                        DrawBond(bond, points[oppHandleIndex], oppHandleIndex);
                    }
                }
                else // align
                {
                    int handleIndex = bond.SourceAttachment.GetHandleIndex();
                    DrawBond(bond, points[handleIndex], handleIndex);
                }
            }
        }
        private void DrawBond(Bond bond, PointF pt, int handleIndex)
        {
            BondAttachment ba = BondAttachmentExtensions.GetTargetFromHandleIndex(handleIndex);
            HandleIcon handleIcon = GetHandleIcon(bond.BondType, TransformKind.Scale);

            if (handleIcon != HandleIcon.Anchor && handleIcon != HandleIcon.Spring)
            {
                float hr = handleSize / 2f;
                int variationIndex = handleIndex % icons[(int)handleIcon].Length;
                Point p = new Point((int)(pt.X - hr), (int)(pt.Y - hr));
                graphicHolder.DrawImage(icons[(int)handleIcon][variationIndex], p);
            }

            long handleHash = bond.GetHandleHash(ba);
            if (bondPoints.ContainsKey(handleHash))
            {
                // todo: need to allow vertical align inside horz dist chain (which will conflict on a handle).
                Console.WriteLine("conflict handle: " + handleHash);
            }
            else
            {
                bondPoints.Add(handleHash, pt);
            }
        }
        public Graphics graphicHolder;
        public void DrawOutlineAndBonds(uint id, float newBoundsLeft, float newBoundsTop)
        {
            DesignInstance di = stage.CurrentEditItem[id];
            PointF diff = new PointF(newBoundsLeft - di.StrokeBounds.Left, newBoundsTop - di.StrokeBounds.Top);

            // draw outlines
            if (diff != PointF.Empty)
            {
                GraphicsState gs = graphicHolder.Save();
                graphicHolder.Transform = stage.CameraMatrix;
                stage.Selection.DrawTranslated(graphicHolder, id, newBoundsLeft, newBoundsTop);
                graphicHolder.Restore(gs);
            }

            // draw bonds
            PointF[] pts = di.GetTransformedPoints(stage.CameraMatrix);

            PointF[] tps = new PointF[] {diff};
            stage.CameraMatrix.TransformVectors( tps );
            pts.TranslatePoints(tps[0].X, tps[0].Y);
            PointF[] bondPoints = pts.GetMidpointsAndCenter();

            DrawBondHandles(stage.CurrentEditItem, bondPoints, id);
        }
        private const int squiggleCount = 8;
        public void DrawBondLines(Graphics g)
        {
            List<Bond> starts = new List<Bond>();
            foreach (Bond b in drawnBonds)
            {
                if (b.ChainType != ChainType.None && b.IsStart)
                {
                    starts.Add(b);
                }
            }

            foreach (Bond start in starts)
            {
                if (start.ChainType.IsDistributed())
                {
                    Bond curBond = start;
                    while (curBond.Next != null)
                    {
                        Bond nextBond = curBond.Next;
                        PointF p0 = bondPoints[curBond.GetHandleHash(curBond.ChainType.GetOppositeAttachment())];
                        PointF p1 = bondPoints[nextBond.GetHandleHash()];
                        float difX = p1.X - p0.X;
                        float difY = p1.Y - p0.Y;
                        float dist = (float)Math.Sqrt(difX * difX + difY * difY);
                        float normX = difX * (1f / dist) * 5;
                        float normY = difY * (1f / dist) * 5;
                        PointF[] pts = new PointF[squiggleCount + 1];
                        pts[0] = p0;
                        pts[pts.Length - 1] = p1;
                        for (int i = 1; i < pts.Length - 1; i++)
                        {
                            int sign = i % 2 == 0 ? 1 : -1;
                            pts[i] = new PointF(
                                p0.X + difX * i / squiggleCount + normY * sign,
                                p0.Y + difY * i / squiggleCount + normX * sign);
                        }
                        PointF midPoint = p0.MidPoint(p1);
                        //g.DrawLine(Pens.Green, p0, p1);
                        g.DrawCurve(Pens.Green, pts);
                        curBond = nextBond;
                    }
                }
                else if (start.ChainType.IsAligned())
                {
                    Bond curBond = start;
                    Bond lastBond = curBond.GetLast();

                    PointF pStart = bondPoints[curBond.GetHandleHash()];
                    PointF pEnd = bondPoints[lastBond.GetHandleHash()];
                    BondLine bl = new BondLine(pStart, pEnd, curBond.BondType, curBond.SourceAttachment);

                    do
                    {
                        PointF p0 = bondPoints[curBond.GetHandleHash()];

                        float xb = p0.X + bl.offset.X;
                        float yb = p0.Y + bl.offset.Y;
                        PointF p0b = new PointF(xb, yb);
                        g.DrawLine(Pens.Red, p0, p0b);

                        curBond = curBond.Next;
                    }
                    while (curBond != null && !curBond.IsStart);

                    // joining line
                    g.DrawLine(
                        Pens.Red,
                        pStart.X + bl.offset.X,
                        pStart.Y + bl.offset.Y,
                        pEnd.X + bl.offset.X,
                        pEnd.Y + bl.offset.Y);
                }
            }

            drawnBonds.Clear();
            bondLines.Clear();
            bondPoints.Clear();
        }


        public void DrawMask(FastBitmap fbmp)
        {
            PointF[] points = stage.TransformPointsCamera;
            PointF[] allPts = points.GetMidpointsAndCenter();

            if (points != null && points.Length == 4)
            {
                PointF center = stage.RotationCenterToCamera;

                ColorMask baseMask = (transformKind == TransformKind.Scale) ? ColorMask.ScaleTopLeft : ColorMask.RotateTopLeft;

                if (AspectConstrainTarget != BondAttachment.None)
                {
                    uint handleIndex = (uint)AspectConstrainTarget.GetHandleIndex();
                    ColorMask cm = (ColorMask)(baseMask + handleIndex);
                    fbmp.AddMaskRect(cm.GetColor(), GetCenterBoundsFromPoint(allPts[handleIndex]));
                }
                else
                {
                    for (uint i = 0; i < 8; i++)
			        {
                        ColorMask cm = (ColorMask)(baseMask + i);
                        fbmp.AddMaskRect(cm.GetColor(), GetCenterBoundsFromPoint(allPts[i]) );			 
			        }

                    if (transformKind == TransformKind.Rotate)
                    {
                        fbmp.AddMaskRect(ColorMask.CenterPoint.GetColor(), GetCenterBoundsFromPoint(allPts[8]));
                    }
                }
            }
        }

        private void DrawOutlineBox(Graphics g, float xDif, float yDif, PointF[] pts)
        {
            float xo, yo;
            for (int i = 0; i < pts.Length - 1; i++)
            {
                if ((i & 2) == 0)
                {
                    xo = xDif * segTableA[i];
                    yo = yDif * segTableB[i];
                }
                else
                {
                    xo = yDif * segTableA[i];
                    yo = xDif * segTableB[i];
                }

                g.DrawLine(selectionRectPen,
                    new PointF(pts[i].X + xo, pts[i].Y + yo),
                    new PointF(pts[i + 1].X - xo, pts[i + 1].Y - yo));
            }
        }

        private static HandleIcon GetHandleIcon(BondType bt, TransformKind tk)
        {
            HandleIcon result;

            switch (bt)
            {
                case BondType.Handle:
                    result = (tk == Enums.TransformKind.Scale) ? HandleIcon.Handle : HandleIcon.Rotate;
                    break;
                case BondType.Join:
                    result = HandleIcon.Join;
                    break;
                case BondType.Anchor:
                    result = HandleIcon.Anchor;
                    break;
                case BondType.Lock:
                    result = HandleIcon.Lock;
                    break;
                case BondType.Pin:
                    result = HandleIcon.Pin;
                    break;
                case BondType.Spring:
                    result = HandleIcon.Spring;
                    break;
                default:
                    result = HandleIcon.Handle;
                    break;
            }

            return result;
        }

        //private static Image handleSquare = Properties.Resources.handle_square;
        private static Image handleSquare = Properties.Resources.handle_square;
        private static Image handleSquareSmall = Properties.Resources.handle_squareSmall;
        private static Image handleRotate = Properties.Resources.handle_rotate;
        private static Image handleRotate0 = Properties.Resources.handle_rotate0;
        private static Image handleRotate1 = Properties.Resources.handle_rotate1;
        private static Image handleRotate2 = Properties.Resources.handle_rotate2;
        private static Image handleRotate3 = Properties.Resources.handle_rotate3;
        private static Image handleAnchor  = Properties.Resources.handle_anchor;
        private static Image handleLock    = Properties.Resources.handle_lock;
        private static Image handleJoin    = Properties.Resources.handle_join;
        private static Image handlePin     = Properties.Resources.handle_pin;
        private static Image handleSpring0 = Properties.Resources.handle_spring0;
        private static Image handleSpring1 = Properties.Resources.handle_spring1;
        private static Image handleSpring2 = Properties.Resources.handle_spring2;
        private static Image handleSpring3 = Properties.Resources.handle_spring3;
        private static Image handleMagnet0 = Properties.Resources.handle_magnet0;
        private static Image handleMagnet1 = Properties.Resources.handle_magnet1;
        private static Image handleMagnet2 = Properties.Resources.handle_magnet2;
        private static Image handleMagnet3 = Properties.Resources.handle_magnet3;
        

        private static Image[][] icons;
        static TransformHandles()
        {
            icons = new Image[(int)HandleIcon.Last][];

            icons[(int)HandleIcon.Handle] = new Image[] { handleSquare, handleSquareSmall };
            icons[(int)HandleIcon.Join]   = new Image[] { handleJoin };
            icons[(int)HandleIcon.Rotate] = new Image[] { handleRotate0, handleRotate, handleRotate1, handleRotate, handleRotate2, handleRotate, handleRotate3, handleRotate };
            icons[(int)HandleIcon.Anchor] = new Image[] { handleAnchor };
            icons[(int)HandleIcon.Lock]   = new Image[] { handleLock };
            icons[(int)HandleIcon.Pin]    = new Image[] { handlePin };
            icons[(int)HandleIcon.Spring] = new Image[] { handleSpring0, handleSpring0, handleSpring1, handleSpring1, handleSpring2, handleSpring2, handleSpring3, handleSpring3 };
            icons[(int)HandleIcon.Magnet] = new Image[] { handleMagnet0, handleMagnet1, handleMagnet2, handleMagnet3 };
        }

        private enum HandleIcon : int
        {
            None,

            Handle,
            Join,
            Rotate,
            Anchor,
            Lock,
            Pin,
            Spring,
            Magnet,

            Last,
        }

        private struct BondLine// : IComparable
        {
            public PointF p0;
            public PointF p1;
            public BondType bondType;
            private BondAttachment attachment;
            public PointF offset;
            private static int offsetDistance = 10;

            public BondLine(PointF p0, PointF p1, BondType bondType, BondAttachment attachment)
            {
                this.bondType = bondType;
                this.attachment = attachment;
                this.p0 = p0;
                this.p1 = p1;
                
                offset = Point.Empty;
                GetOffset(p0, p1);
            }

            private void GetOffset(PointF p0, PointF p1)
            {
                if (bondType != BondType.Spring) // spring lines are direct, not offest from icon
                {
                    bool needsRotate = false;
                    switch (attachment)
                    {
                        case BondAttachment.ObjectHandleT:
                        case BondAttachment.ObjectHandleB:
                            needsRotate = ((int)p0.Y != (int)p1.Y);
                            break;
                        case BondAttachment.ObjectHandleL:
                        case BondAttachment.ObjectHandleR:
                            needsRotate = ((int)p0.X != (int)p1.X);
                            break;
                    }

                    if (needsRotate)
                    {
                        double rotation = attachment.IsBottomOrLeft() ? Math.PI * 0.5d : -Math.PI * 0.5d;
                        double angle = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X) + rotation;
                        float x = (float)(offsetDistance * Math.Cos(angle));
                        float y = (float)(offsetDistance * Math.Sin(angle));
                        offset = new PointF(x, y);
                    }
                    else
                    {
                        switch (attachment)
                        {
                            case BondAttachment.ObjectHandleT:
                                offset = new Point(0, -offsetDistance);
                                break;
                            case BondAttachment.ObjectHandleR:
                                offset = new Point(offsetDistance, 0);
                                break;
                            case BondAttachment.ObjectHandleB:
                                offset = new Point(0, offsetDistance);
                                break;
                            case BondAttachment.ObjectHandleL:
                                offset = new Point(-offsetDistance, 0);
                                break;
                            default:
                                offset = Point.Empty;
                                break;
                        }
                    }
                }
            }

       }
    }

}
