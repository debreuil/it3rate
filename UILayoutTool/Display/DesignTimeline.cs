using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DDW.Managers;
using DDW.Interfaces;
using DDW.Commands;
using Vex = DDW.Vex;
using DDW.Enums;
using DDW.Assets;
using DDW.Utils;
using DDW.Views;
using DDW.Controls;
using DDW.Vex.Bonds;

namespace DDW.Display
{
    public class DesignTimeline : DesignInstance
    {
        private Vex.Timeline timeline { get { return (Vex.Timeline)this.Definition; } }

        public SnapStore SnapStore;
        public BondStore BondStore;
        public Guidelines Guidelines;

        public bool isRoot;

        public DesignTimeline(StageView stage, Vex.Instance inst) : base(stage, inst)
        {
            selectedItems = new InstanceGroup(stage.InstanceManager);

            SnapStore = new SnapStore(this);
            BondStore = new BondStore(this);
            Guidelines = new Guidelines(this);
        }

        public Size LocalCanvasSize { get { return stage.CanvasSize; } } // todo: make local with MX etc

        public DesignInstance this[uint index]
        {
            get{return stage.InstanceManager[index];}
        }
        
        public void EnsureSnaps()
        {
            if (!SnapStore.HasSnaps && (Guidelines.Count > 0 || this.Count > 0))
            {
                uint[] instIds = InstanceIds;
                for (int i = 0; i < instIds.Length; i++)
                {
                    DesignInstance di = stage.InstanceManager[instIds[i]];
                    SnapStore.AddInstance(di);                    
                }

                Guide[] guides = Guidelines.Guides;
                for (int i = 0; i < guides.Length; i++)
                {
                    SnapStore.AddInstance(guides[i]);
                }
            }
        }

        public bool CanAdd(LibraryItem li)
        {
            bool result = true;
            if (li.DefinitionId == this.LibraryItem.DefinitionId)
            {
                result = false;
            }
            else if(li.Definition is Vex.Timeline)
            {
                uint[] instIds = ((Vex.Timeline)li.Definition).GetInstanceIds();
                for (int i = 0; i < instIds.Length; i++)
			    {
			        LibraryItem li2 = stage.InstanceManager[instIds[i]].LibraryItem;
                    if(!CanAdd(li2))
                    {
                        result = false;
                        break;
                    }
			    }
            }
            return result;
        }

        public void DrawMask(Graphics g)
        {
            // may never be used in DesignInstance (parent class), 
            // for now only designTimelines can be rooted.
            // Would be useful in DesignInstance if vector editor comes later

            // note this is not recursive, as we only need the mask for the current edit layer
            //Matrix orgM = g.Transform;
            //Matrix newM = GetMatrix().SysMatrix();
            //newM.Multiply(orgM, MatrixOrder.Append);
            //g.Transform = newM;

            foreach (uint id in InstanceIds)
            {
                Color c = Color.FromArgb((int)(0xFF000000 + id));
                this[id].DrawMaskInto(g, c);
                //designStage[id].DrawMask(g, c);
            }

            //newM.Dispose();
            //g.Transform = orgM;
        }

        #region Elements
        public uint[] InstanceIds { get { return timeline.GetInstanceIds(); } }
        public int Count { get { return timeline.InstanceCount; } }
        public bool Contains(uint index)
        {
            return timeline.ContainsInstanceId(index);
        }
        public DesignInstance Add(uint definitionId, Vex.Point location)
        {
            DesignInstance si = stage.CreateInstance(definitionId, location);
            if (si != null)
            {
                timeline.AddInstance(si.Instance);
                si.ParentInstanceId = InstanceHash;
                selectedItems.Clear();
                selectedItems.Add(si.InstanceHash);
                SnapStore.AddInstance(si);
            }
            return si;
        }
        public uint[] AddRange(uint[] libraryIds, Vex.Point[] locations)
        {
            uint[] selIds = new uint[libraryIds.Length];
            for (int i = 0; i < libraryIds.Length; i++)
            {
                DesignInstance si = stage.CreateInstance(libraryIds[i], locations[i]);
                if (si != null)
                {
                    timeline.AddInstance(si.Instance);
                    si.ParentInstanceId = InstanceHash;
                    selIds[i] = si.InstanceHash;
                    SnapStore.AddInstance(si);
                }
            }
            return selIds;
        }
        public void AddInstancesById(uint[] instanceIds)
        {
            foreach (uint id in instanceIds)
            {
                if (!timeline.ContainsInstanceId(id) && stage.InstanceManager.IsRemovedInstance(id))
                {
                    stage.InstanceManager.CancelRemove(id);
                    timeline.AddInstance(stage.InstanceManager[id].Instance);
                    stage.InstanceManager[id].ParentInstanceId = InstanceHash;
                    SnapStore.AddInstance(stage.InstanceManager[id]);
                }
            }
        }
        public DesignInstance Remove(uint instId)
        {
            selectedItems.Remove(instId);
            DesignInstance di = stage.InstanceManager[instId];
            Vex.IInstance inst = di.Instance;
            timeline.RemoveInstance(inst);
            stage.InstanceManager.RemoveInstance(di);
            SnapStore.RemoveInstance(di);
            BondStore.RemoveBondsForInstance(instId);
            return di;
        }
        public DesignInstance[] RemoveInstancesById(uint[] instanceIds)
        {
            DesignInstance[] removedInstances = new DesignInstance[instanceIds.Length];
            for (int i = 0; i < instanceIds.Length; i++)
            {
                uint id = instanceIds[i];
                removedInstances[i] = Remove(id);
            }
            selectedItems.Clear();
            return removedInstances;
        }
        public void InsertExistingOnUndo(int depth, DesignInstance di)
        {
            depth = Math.Max(0, Math.Min(timeline.InstanceCount, depth));
            timeline.InsertInstance(depth, di.Instance);
            if (!stage.InstanceManager.Contains(di.InstanceHash))
            {
                stage.InstanceManager.AddInstance(di);
            }

            di.ParentInstanceId = InstanceHash; 
            SnapStore.AddInstance(di);
        }



        public uint[] SortInPlaceLeftToRight(uint[] ids)
        {
            return ids.OrderBy(item => stage.InstanceManager[item].Location.X).ToArray();
        }
        public uint[] SortInPlaceTopToBottom(uint[] ids)
        {
            return ids.OrderBy(item => stage.InstanceManager[item].Location.Y).ToArray();
        }
        #endregion

        #region Selection
        private InstanceGroup selectedItems;
        public InstanceGroup Selected { get { return selectedItems; } }

        public int GetDepth(uint id)
        {
            return timeline.GetInstanceDepth(id);
        }
        public void SwapDepths(int[] from, int[] to)
        {
            timeline.SwapDepths(from, to);
            HasSaveableChanges = true;
        }
        public void ChangeDepth(int from, int to)
        {
            timeline.ChangeDepth(from, to);
            HasSaveableChanges = true;
        }
        #endregion

        public override void Draw(Graphics g)
        {
            if (stage.ShowGuides)
            {
                bool focused = (stage.IsEditingRoot == isRoot);
                Guidelines.Draw(g, focused);
            }
            base.Draw(g);
        }
    }
}
