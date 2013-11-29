using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using System.Drawing;
using DDW.Views;
using Vex = DDW.Vex;
using DDW.Display;
using DDW.Managers;
using DDW.Enums;
using DDW.Vex.Bonds;

namespace DDW.Commands
{
    class AlignCommand : ICommand, ISaveableCommand
    {
        Vex.Point[] offsets;
        List<Bond> addedBonds = new List<Bond>();
        List<Bond> previousBonds;
        bool useSmartBonds;

        ChainType chainType;
        public ChainType ChainType { get { return chainType; } }

        public AlignCommand(ChainType chainType)
        {
            this.chainType = chainType;
            previousBonds = new List<Bond>();
            useSmartBonds = MainForm.CurrentStage.UseSmartBonds;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            Vex.Rectangle bounds = sel.StrokeBounds;
            uint[] ids = sel.SelectedIds;
            offsets = new Vex.Point[ids.Length];
            
            InstanceManager im = MainForm.CurrentInstanceManager;
            List<uint> sortIds = new List<uint>(ids);
            im.SortIndexesByLocation(sortIds, chainType);

            Vex.Point target = Vex.Point.Empty;

            if (chainType == ChainType.DistributedHorizontal)
            {
                float firstWidth = MainForm.CurrentInstanceManager[sortIds[0]].StrokeBounds.Width;
                float lastWidth = MainForm.CurrentInstanceManager[sortIds[sortIds.Count - 1]].StrokeBounds.Width;
                float fillWidth = bounds.Width - firstWidth - lastWidth;
                float remainingWidths = 0;
                for (int i = 1; i < sortIds.Count - 1; i++)
                {
                    DesignInstance di = MainForm.CurrentInstanceManager[sortIds[i]];
                    remainingWidths += di.StrokeBounds.Width;
                }

                float spacing = (fillWidth - remainingWidths) / (sortIds.Count - 1);
                float curLoc = bounds.Left;

                for (int i = 0; i < sortIds.Count; i++)
                {
                    DesignInstance di = MainForm.CurrentInstanceManager[sortIds[i]];
                    Vex.Point offset = new Vex.Point(curLoc - di.StrokeBounds.Left, 0);

                    Vex.Matrix m = di.GetMatrix();
                    m.Translate(offset);
                    stage.SetDesignInstanceMatrix(di, m);

                    curLoc += spacing + di.StrokeBounds.Width;

                    int realIndex = Array.IndexOf(ids, sortIds[i]);
                    offsets[realIndex] = offset;
                }

                target = new Vex.Point(spacing, float.NaN);
            }
            else if (chainType == ChainType.DistributedVertical)
            {
                float firstHeight = MainForm.CurrentInstanceManager[sortIds[0]].StrokeBounds.Height;
                float lastHeight = MainForm.CurrentInstanceManager[sortIds[sortIds.Count - 1]].StrokeBounds.Height;
                float fillHeight = bounds.Height - firstHeight - lastHeight;
                float remainingHeights = 0;
                for (int i = 1; i < sortIds.Count - 1; i++)
                {
                    DesignInstance di = MainForm.CurrentInstanceManager[sortIds[i]];
                    remainingHeights += di.StrokeBounds.Height;
                }

                float spacing = (fillHeight - remainingHeights) / (sortIds.Count - 1);
                float curLoc = bounds.Top;

                for (int i = 0; i < sortIds.Count; i++)
                {
                    DesignInstance di = MainForm.CurrentInstanceManager[sortIds[i]];
                    Vex.Point offset = new Vex.Point(0, curLoc - di.StrokeBounds.Top);

                    Vex.Matrix m = di.GetMatrix();
                    m.Translate(offset);
                    stage.SetDesignInstanceMatrix(di, m);

                    curLoc += spacing + di.StrokeBounds.Height;

                    int realIndex = Array.IndexOf(ids, sortIds[i]);
                    offsets[realIndex] = offset;
                }
                target = new Vex.Point(float.NaN, spacing);

            }
            else
            {
                Vex.Rectangle contrainedBounds = bounds.Clone();
                stage.CurrentEditItem.BondStore.ConstrainBounds(sortIds, ref contrainedBounds, chainType);

                switch (chainType)
                {
                    case ChainType.AlignedLeft:
                        target = new Vex.Point(contrainedBounds.Left, float.NaN);
                        break;
                    case ChainType.AlignedCenterVertical:
                        target = new Vex.Point(contrainedBounds.Center.X, float.NaN);
                        break;
                    case ChainType.AlignedRight:
                        target = new Vex.Point(contrainedBounds.Right, float.NaN);
                        break;
                    case ChainType.AlignedTop:
                        target = new Vex.Point(float.NaN, contrainedBounds.Top);
                        break;
                    case ChainType.AlignedCenterHorizontal:
                        target = new Vex.Point(float.NaN, contrainedBounds.Center.Y);
                        break;
                    case ChainType.AlignedBottom:
                        target = new Vex.Point(float.NaN, contrainedBounds.Bottom);
                        break;
                }

                for (int i = 0; i < sortIds.Count; i++)
                {
                    DesignInstance di = MainForm.CurrentInstanceManager[sortIds[i]];
                    Vex.Point offset = Vex.Point.Zero;

                    switch (chainType)
                    {
                        case ChainType.AlignedLeft:
                            offset.X = target.X - di.StrokeBounds.Left;
                            break;
                        case ChainType.AlignedCenterVertical:
                            offset.X = target.X - (di.StrokeBounds.Left + di.StrokeBounds.Width / 2);
                            break;
                        case ChainType.AlignedRight:
                            offset.X = target.X - (di.StrokeBounds.Left + di.StrokeBounds.Width);
                            break;
                        case ChainType.AlignedTop:
                            offset.Y = target.Y - di.StrokeBounds.Top;
                            break;
                        case ChainType.AlignedCenterHorizontal:
                            offset.Y = target.Y - (di.StrokeBounds.Top + di.StrokeBounds.Height / 2);
                            break;
                        case ChainType.AlignedBottom:
                            offset.Y = target.Y - (di.StrokeBounds.Top + di.StrokeBounds.Height);
                            break;
                    }

                    Vex.Matrix m = di.GetMatrix();
                    m.Translate(offset);
                    stage.SetDesignInstanceMatrix(di, m);

                    offsets[i] = offset;
                }

            }

            if (useSmartBonds)
            {
                stage.CurrentEditItem.BondStore.Align(sortIds.ToArray(), chainType, target, addedBonds, previousBonds);
            }

            sel.Update();
            MainForm.CurrentStage.ResetTransformHandles();
            MainForm.CurrentStage.InvalidateTransformedSelection();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            uint[] ids = sel.SelectedIds;

            foreach (Bond b in addedBonds)
            {
                stage.CurrentEditItem.BondStore.RemoveBond(b);
            }

            for (int i = 0; i < ids.Length; i++)
            {
                DesignInstance di = MainForm.CurrentInstanceManager[ids[i]];

                Vex.Matrix m = di.GetMatrix();
                Vex.Point offset = new Vex.Point(-offsets[i].X, -offsets[i].Y);
                m.Translate(offset);
                stage.SetDesignInstanceMatrix(di, m);
            }

            if (useSmartBonds)
            {
                foreach (Bond b in previousBonds)
                {
                    stage.CurrentEditItem.BondStore.AddBond(b);
                }
            }

            sel.Update();
            MainForm.CurrentStage.ResetTransformHandles();
            MainForm.CurrentStage.InvalidateTransformedSelection();
        }

        public void MarkSaveableChanges()
        {
            // new instances are always marked as needing save
            MainForm.CurrentStage.HasSaveableChanges = true;
        }
    }
}
