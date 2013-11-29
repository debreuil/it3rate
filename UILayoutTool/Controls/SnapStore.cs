using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vex = DDW.Vex;
using DDW.Vex.Bonds;
using DDW.Enums;
using DDW.Display;
using DDW.Utils;
using System.Drawing;
using DDW.Data;

namespace DDW.Controls
{
    public class SnapStore
    {
        private DesignTimeline timeline;

        private SortedList<float, List<ICrossPoint>> xStops;
        private SortedList<float, List<ICrossPoint>> yStops;

        public readonly int unzoomedSnapDistance = 10;
        public int SnapDistance { get; set; }
        public bool HasSnaps { get { return xStops.Count > 0 || yStops.Count > 0; } }

        public SnapStore(DesignTimeline timeline)
        {
            this.timeline = timeline;
            xStops = new SortedList<float, List<ICrossPoint>>();
            yStops = new SortedList<float, List<ICrossPoint>>();
        }

        public ICrossPoint[] GetCrossPoint(Vex.Point snapPoint, BondAttachment ba)
        {
            List<ICrossPoint> result = new List<ICrossPoint>();
            if (ba.IsHandle())
            {
                if (xStops.ContainsKey(snapPoint.X)) // will not contain point if snaping to original position
                {
                    List<ICrossPoint> cps = xStops[snapPoint.X];
                    ICrossPoint cp = cps.Find(item => item.CrossStart == snapPoint.Y);
                    if (cp != null)
                    {
                        result.Add(cp);
                    }
                }
            }
            else
            {
                if (ba.IsVGuide())
                {
                    List<ICrossPoint> cps = xStops[snapPoint.X];
                    if (cps.Count > 0)
                    {
                        result.Add(cps[0]);
                    }
                }
                if (ba.IsHGuide())
                {
                    List<ICrossPoint> cps = yStops[snapPoint.Y];
                    if (cps.Count > 0)
                    {
                        result.Add(cps[0]);
                    }
                }
            }
            return result.ToArray();
        }
        public void AddInstance(Guide guide)
        {
            uint id = guide.InstanceHash;
            switch (guide.GuideType)
            {
                case GuideType.Horizontal:
                    AddPoint(yStops, guide.StartPoint.Y, new CrossRange(guide.StartPoint.X, guide.EndPoint.X, id, BondAttachment.HGuide));
                    break;
                case GuideType.Vertical:
                    AddPoint(xStops, guide.StartPoint.X, new CrossRange(guide.StartPoint.Y, guide.EndPoint.Y, id, BondAttachment.VGuide));
                    break;
                case GuideType.Rectangle:
                    AddPoint(xStops, guide.StartPoint.X, new CrossRange(guide.StartPoint.Y, guide.EndPoint.Y, id, BondAttachment.VGuide));
                    AddPoint(yStops, guide.StartPoint.Y, new CrossRange(guide.StartPoint.X, guide.EndPoint.X, id-1, BondAttachment.HGuide));

                    AddPoint(xStops, guide.EndPoint.X, new CrossRange(guide.StartPoint.Y, guide.EndPoint.Y, id-2, BondAttachment.VGuide));
                    AddPoint(yStops, guide.EndPoint.Y, new CrossRange(guide.StartPoint.X, guide.EndPoint.X, id-3, BondAttachment.HGuide));
                    break;
                case GuideType.Point:
                    AddPoint(xStops, guide.StartPoint.X, new CrossPoint(guide.StartPoint.Y, id, BondAttachment.GridPoint));
                    AddPoint(yStops, guide.StartPoint.Y, new CrossPoint(guide.StartPoint.X, id-1, BondAttachment.GridPoint));
                    break;
            }
        }        
        public void RemoveInstance(Guide guide)
        {
            uint hash = guide.InstanceHash;

            bool success = true;
            switch (guide.GuideType)
            {
                case GuideType.Horizontal:
                    success = RemoveLine(yStops, guide.StartPoint.Y, guide.StartPoint.X, guide.EndPoint.X, hash);
                    break;
                case GuideType.Vertical:
                    success = RemoveLine(xStops, guide.StartPoint.X, guide.StartPoint.Y, guide.EndPoint.Y, hash);
                    break;
                case GuideType.Rectangle:
                    success = RemoveLine(xStops, guide.StartPoint.X, guide.StartPoint.Y, guide.EndPoint.Y, hash);
                    success &= RemoveLine(yStops, guide.StartPoint.Y, guide.StartPoint.X, guide.EndPoint.X, hash - 1);
                    success &= RemoveLine(xStops, guide.EndPoint.X,   guide.StartPoint.Y, guide.EndPoint.Y, hash - 2);
                    success &= RemoveLine(yStops, guide.EndPoint.Y,   guide.StartPoint.X, guide.EndPoint.X, hash - 3);
                    break;
                case GuideType.Point:
                    success = RemovePoint(xStops, guide.StartPoint.X, guide.StartPoint.Y, hash);
                    success &= RemovePoint(yStops, guide.StartPoint.Y, guide.StartPoint.X, hash - 1);
                    break;
            }

            if (!success)
            {
                RemoveByIndex(xStops, hash);
                RemoveByIndex(yStops, hash);
            }
        }
        public void AddInstance(DesignInstance inst)
        {
            PointF[] pts = (inst.IsRotated) ? inst.GetTransformedCenter() : inst.GetTransformedPoints().GetMidpointsAndCenter();

            for (int i = 0; i < pts.Length; i++)
            {
                BondAttachment ba = BondAttachmentExtensions.GetTargetFromHandleIndex(i);
                AddPoint(xStops, pts[i].X, new CrossPoint(pts[i].Y, inst.InstanceHash, ba));
                AddPoint(yStops, pts[i].Y, new CrossPoint(pts[i].X, inst.InstanceHash, ba));
            }
        }
        public void RemoveInstance(DesignInstance inst)
        {
            PointF[] pts = (inst.IsRotated) ? inst.GetTransformedCenter() : inst.GetTransformedPoints().GetMidpointsAndCenter();

            uint hash = inst.InstanceHash;
            bool success = true;
            for (int i = 0; i < pts.Length; i++)
            {
                bool successX = RemovePoint(xStops, pts[i].X, pts[i].Y, hash);
                bool successY = RemovePoint(yStops, pts[i].Y, pts[i].X, hash);
                if (!successX || !successY)
                {
                    success = false;
                    break;
                }
            }

            if (!success)
            {
                RemoveByIndex(xStops, hash);
                RemoveByIndex(yStops, hash);
            }
        }

        private void AddPoint(SortedList<float, List<ICrossPoint>> store, float key, ICrossPoint cr)
        {
            if(!store.ContainsKey(key))
            {
                store.Add(key, new List<ICrossPoint>(){cr});
            }
            else
            {
                store[key].Add(cr);
            }
        }
        private bool RemoveLine(SortedList<float, List<ICrossPoint>> store, float key, float valStart, float valEnd, uint instanceHash)
        {
            bool result = false;
            if (store.ContainsKey(key))
            {
                int index = store[key].FindIndex(cp => cp.InstanceHash == instanceHash && cp.CrossStart == valStart && cp.CrossEnd == valEnd);
                if (index > -1)
                {
                    store[key].RemoveAt(index);
                    result = true;
                }
            }
            return result;
        }
        private bool RemovePoint(SortedList<float, List<ICrossPoint>> store, float key, float val, uint instanceHash)
        {
            bool result = false;
            if (store.ContainsKey(key))
            {
                int index = store[key].FindIndex(cp => cp.InstanceHash == instanceHash && cp.CrossStart == val);
                if (index > -1)
                {
                    store[key].RemoveAt(index);
                    result = true;
                }
            }
            return result;
        }
        private void RemoveByIndex(SortedList<float, List<ICrossPoint>> store, uint instanceHash)
        {
            // slower, used as last resort if floating point errors are too large
            for(int i = store.Keys.Count - 1; i >=0; i--)
            {
                float key = store.Keys[i];
                store[key].RemoveAll(icp => icp.InstanceHash == instanceHash);
                if (store[key].Count == 0)
                {
                    store.Remove(key);
                }
            }
        }

        public Vex.Point GetSnapPoints(PointF[] pts, ref Vex.Point[] snapPoints, ref BondAttachment[] snapTypes)
        {
            Vex.Point resultOffset = Vex.Point.Zero;

            // get first snap point for each handle point (based on top left)
            Vex.Point[] snapsXP = new Vex.Point[pts.Length];
            Vex.Point[] snapsY = new Vex.Point[pts.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                snapsXP[i] = Vex.Point.Empty;
                snapsY[i] = Vex.Point.Empty;
                GetSnapPoint(xStops, pts[i].X, pts[i].Y, ref snapsXP[i].X, ref snapsXP[i].Y);
                GetSnapPoint(yStops, pts[i].Y, pts[i].X, ref snapsY[i].Y, ref snapsY[i].X, false);
            }

            // get closest X and Y
            float bestDist = float.MaxValue;
            float bestDistX = float.MaxValue;
            float bestDistY = float.MaxValue;
            Vex.Point bestPoint = Vex.Point.Empty;

            Dictionary<int, BondAttachment> bestIndexes = new Dictionary<int, BondAttachment>();
            for (int i = 0; i < snapsXP.Length; i++)
            {
                // find the distance to the snap(s) for the given handle
                Vex.Point rp = snapsXP[i];
                float difX = 0;
                float difY = 0;
                BondAttachment targ = BondAttachment.None;
                if(!float.IsNaN(rp.X) && !float.IsNaN(rp.Y)) // point
                {
                    targ = BondAttachment.SymbolHandle;
                    difX = rp.X - pts[i].X;
                    difY = rp.Y - pts[i].Y;
                }
                else if (!float.IsNaN(rp.X)) // vertical guide
                {
                    targ = BondAttachment.VGuide;
                    difX = rp.X - pts[i].X;
                }
                rp = snapsY[i];
                if (float.IsNaN(rp.X) && !float.IsNaN(rp.Y)) // horizontal guide
                {
                    targ = (targ == BondAttachment.VGuide) ? BondAttachment.CornerGuide : BondAttachment.HGuide;
                    difY = rp.Y - pts[i].Y;
                }


                // check if close enough to win or tie
                float distX = difX * difX;
                float distY = difY * difY;
                if (targ.IsHandle())
                {
                    float dist = distX + distY;
                    if (dist <= bestDist + 0.001f)
                    {
                        if (dist + 0.001f < bestDist &&
                            (bestPoint.IsEmpty || Math.Abs(difX - bestPoint.X) < 0.001 || Math.Abs(difY - bestPoint.Y) < 0.001))
                        {
                            bestIndexes.Clear();
                            bestDist = dist;
                            bestPoint = new Vex.Point(difX, difY);
                            bestDistX = distX;
                            bestDistY = distY;
                            bestIndexes.Add(i, targ);
                        }
                        else if(Math.Abs(distX - bestDistX) < 0.001 && Math.Abs(distY - bestDistY) < 0.001)
                        {
                            bestIndexes.Add(i, targ);
                        }
                    }
                }
                else if (targ != BondAttachment.None)  // guides
                {
                    float minXY = Math.Min(distX, distY);

                    if (minXY < bestDist - 0.0001f)
                    {
                        foreach (var item in bestIndexes.Where(kpv => kpv.Value == BondAttachment.SymbolHandle).ToList())
                            { bestIndexes.Remove(item.Key); }
                        bestDist = minXY;
                    }

                    if (targ == BondAttachment.CornerGuide)
                    {
                        if (distX - 0.0001f <= bestDistX || difY - 0.0001f <= bestDistY)
                        {
                            foreach (var item in bestIndexes.Where(kpv =>
                                (   (kpv.Value == BondAttachment.VGuide && bestDistX > distX) || (kpv.Value == BondAttachment.HGuide && bestDistY > distY))
                                ).ToList()) { bestIndexes.Remove(item.Key); }

                            bestDistX = Math.Min(bestDistX, distX);
                            bestDistY = Math.Min(bestDistY, distY);
                            bestIndexes.Add(i, targ);
                        }
                    }
                    else if (targ == BondAttachment.VGuide) // clear non HGuides
                    {
                        if (distX - 0.0001f <= bestDistX)
                        {
                            foreach (var item in bestIndexes.Where(kpv => (kpv.Value.IsVGuide() && bestDistX > distX)).ToList())
                            { bestIndexes.Remove(item.Key); }

                            bestDistX = distX;
                            bestIndexes.Add(i, targ);
                        }
                    }
                    else if (targ == BondAttachment.HGuide) // clear non VGuides
                    {
                        if (distY - 0.0001f <= bestDistY)
                        {
                            foreach (var item in bestIndexes.Where(kpv => (kpv.Value.IsHGuide() && bestDistY > distY)).ToList())
                            { bestIndexes.Remove(item.Key); }

                            bestDistY = distY;
                            bestIndexes.Add(i, targ);
                        }
                    }
                }
            }
            
            // find offset
            bool hasSnap = false;
            Vex.Point target = Vex.Point.Empty;
            foreach(int key in bestIndexes.Keys)
			{
                hasSnap = true;
                BondAttachment bt = bestIndexes[key];
                PointF src = pts[key];
                target = (bt == BondAttachment.HGuide) ? snapsY[key] : snapsXP[key];
                if (bt.IsHandle())
                {
                    resultOffset = new Vex.Point(target.X - src.X, target.Y - src.Y);
                    break;
                }
                else if (bt == BondAttachment.CornerGuide)
                {
                    resultOffset = new Vex.Point(target.X - src.X, snapsY[key].Y - src.Y);
                    break;
                }
                else if (bt == BondAttachment.HGuide)
                {
                    if (resultOffset.X != 0)
                    {
                        resultOffset = new Vex.Point(resultOffset.X, target.Y - src.Y);
                        break;
                    }
                    resultOffset = new Vex.Point(0, target.Y - src.Y);
                }
                else if (bt == BondAttachment.VGuide)
                {
                    if (resultOffset.Y != 0)
                    {
                        resultOffset = new Vex.Point(target.X - src.X, resultOffset.Y);
                        break;
                    }
                    resultOffset = new Vex.Point(target.X - src.X, 0);
                }
			}

            // set snap points
            if (hasSnap)
            {
                foreach (int key in bestIndexes.Keys)
                {
                    BondAttachment bt = bestIndexes[key];
                    snapTypes[key] = bt;

                    target = (bt == BondAttachment.HGuide) ? snapsY[key] : snapsXP[key];
                    if (bt.IsHandle())
                    {
                        snapPoints[key] = new Vex.Point(target.X, target.Y);
                    }
                    else if (bt == BondAttachment.HGuide)
                    {
                        snapPoints[key] = new Vex.Point(pts[key].X + resultOffset.X, target.Y);
                    }
                    else if (bt == BondAttachment.VGuide)
                    {
                        snapPoints[key] = new Vex.Point(target.X, pts[key].Y + resultOffset.Y);
                    }
                    else if (bt == BondAttachment.CornerGuide)
                    {
                        snapPoints[key] = new Vex.Point(pts[key].X + resultOffset.X, pts[key].Y + resultOffset.Y);
                    }
                }
            }

            return resultOffset;
        }
        private void GetSnapPoint(SortedList<float, List<ICrossPoint>> stops, float val, float cross, ref float resultKey, ref float resultCross, bool testPoints = true)
        {
            IList<float> keys = stops.Keys;
            bool snapToPoints = MainForm.CurrentStage.SnapToObjects;
            bool snapToRanges = MainForm.CurrentStage.ShowGuides;

            int start = keys.FindFirstAbove(val - SnapDistance);
            if (start > -1)
            {
                for (int i = start; i < keys.Count; i++)
                {
                    float key = keys[i];
                    if (key >= val - SnapDistance && key <= val + SnapDistance)
                    {
                        List<ICrossPoint> list = stops[key];
                        for (int j = 0; j < list.Count; j++)
                        {
                            // CrossPoint
                            if (snapToPoints && testPoints && list[j] is CrossPoint &&
                                cross > list[j].CrossStart - SnapDistance &&
                                cross < list[j].CrossStart + SnapDistance)
                            {
                                resultKey = key;
                                resultCross = list[j].CrossStart;
                                goto EndLoops;
                            }
                            // CrossRange
                            else if (snapToRanges && list[j] is CrossRange && cross > list[j].CrossStart && cross < list[j].CrossEnd)
                            {
                                resultKey = key;
                                //resultCross = cross;
                                goto EndLoops;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        EndLoops:
            return;
        }


        public static int guideRadius;
        public static Bitmap[] guideIcons;
        public static Bitmap guidePoint = Properties.Resources.guidePoint;
        public static Bitmap guideHorizontal = Properties.Resources.guideHorizontal;
        public static Bitmap guideVertical = Properties.Resources.guideVertical;
        public static Bitmap guideBoth = Properties.Resources.guideBoth;

        public static Bitmap GetSnapIcon(BondAttachment bondAttachment)
        {
            Bitmap result = guideIcons[0];
            switch (bondAttachment)
            {
                case BondAttachment.HGuide:
                    result = guideIcons[1];
                    break;
                case BondAttachment.VGuide:
                    result = guideIcons[2];
                    break;
                case BondAttachment.CornerGuide:
                    result = guideIcons[3];
                    break;
            }
            return result;
        }

        static SnapStore()
        {
            guideRadius = (int)(guideHorizontal.Width / 2f);
            guideIcons = new Bitmap[] { guidePoint, guideHorizontal, guideVertical, guideBoth };
        }
    }

}
