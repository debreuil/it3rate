using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using DDW.Enums;
using DDW.Vex.Bonds;
using Vex = DDW.Vex;
using DDW.Managers;
using DDW.Utils;
using System.Drawing;

namespace DDW.Controls
{
    public class BondStore
    {
        public delegate void BondAction(uint id, float tx, float ty);

        private DesignTimeline timeline;

        private Dictionary<uint, List<Bond>> bonds;
        private Dictionary<uint, BondType[]> handles;
        private Dictionary<uint, List<Bond>> guideAttachments;

        private Bond[] emptyBonds = new Bond[] { };
        
        public BondStore(DesignTimeline timeline)
        {
            this.timeline = timeline;
            this.bonds = new Dictionary<uint, List<Bond>>();
            this.handles = new Dictionary<uint, BondType[]>();
            this.guideAttachments = new Dictionary<uint, List<Bond>>();
        }
        
        public void AddBond(Bond bond)
        {
            if (!bonds.ContainsKey(bond.SourceInstanceId))
            {
                bonds.Add(bond.SourceInstanceId, new List<Bond>());
                handles.Add(bond.SourceInstanceId, (BondType[])emptyHandles.Clone());
            }
            bonds[bond.SourceInstanceId].Add(bond);

            if (bond.ChainType.IsDistributed())
            {
                int handleIndex = bond.ChainType.GetAttachment().GetHandleIndex();
                handles[bond.SourceInstanceId][handleIndex] = bond.BondType;
                handleIndex = bond.ChainType.GetOppositeAttachment().GetHandleIndex();
                handles[bond.SourceInstanceId][handleIndex] = bond.BondType;
            }
            else
            {
                int handleIndex = bond.SourceAttachment.GetHandleIndex();
                handles[bond.SourceInstanceId][handleIndex] = bond.BondType;
            }

            if (bond.Previous != null)
            {
                Bond pBond = bond.Previous;
                bond.Next = pBond.Next;
                pBond.Next = bond;
            }

            if (bond.Next != null)
            {
                Bond nBond = bond.Next;
                bond.Previous = nBond.Previous;
                nBond.Previous = bond;
            }
        }
        public void RemoveBonds(Bond[] delBonds)
        {
            for (int i = 0; i < delBonds.Length; i++)
            {
                RemoveBond(delBonds[i]);
            }
        }
        public void RemoveBond(Bond b)
        {
            if (b.Next != null && b.Next.SourceAttachment.IsGuide())
            {
                guideAttachments[b.Next.SourceInstanceId].Remove(b);
            }
            else if (!b.SourceAttachment.IsGuide() && bonds.ContainsKey(b.SourceInstanceId))
            {
                Bond orphan = b.RemoveSelf();
                if (orphan != null)
                {
                    RemoveBond(orphan);
                }
            }

            if (bonds.ContainsKey(b.SourceInstanceId)) // may have been an orphan
            {
                bonds[b.SourceInstanceId].Remove(b);

                if (b.ChainType.IsDistributed())
                {
                    int handleIndex = b.ChainType.GetAttachment().GetHandleIndex();
                    handles[b.SourceInstanceId][handleIndex] = BondType.Handle;
                    handleIndex = b.ChainType.GetOppositeAttachment().GetHandleIndex();
                    handles[b.SourceInstanceId][handleIndex] = BondType.Handle;
                }
                else if (handles.ContainsKey(b.SourceInstanceId))
                {
                    int handleIndex = b.SourceAttachment.GetHandleIndex();
                    handles[b.SourceInstanceId][handleIndex] = BondType.Handle;
                }

                if (bonds[b.SourceInstanceId].Count == 0)
                {
                    bonds.Remove(b.SourceInstanceId);
                    handles.Remove(b.SourceInstanceId);
                }
            }
        }
        public void RemoveBondsForInstances(uint[] instanceIds)
        {
            for (int i = 0; i < instanceIds.Length; i++)
            {
                RemoveBondsForInstance(instanceIds[i]);
            }
        }
        public void RemoveBondsForInstance(uint instanceId)
        {
            if (bonds.ContainsKey(instanceId))
            {
                Bond[] instBonds = bonds[instanceId].ToArray();
                RemoveBonds(instBonds);
            }
        }
        private Bond GetOrCreateGuideBond(uint instanceId, BondAttachment attachment)
        {
            if (!bonds.ContainsKey(instanceId))
            {
                Bond b = new Bond(instanceId, attachment, BondType.Lock);
                bonds.Add(instanceId, new List<Bond>() { b });
            }

            if (!guideAttachments.ContainsKey(instanceId))
            {
                guideAttachments.Add(instanceId, new List<Bond>());
            }

            return bonds[instanceId][0];
        }
        private bool RemoveCollidingBond(uint instanceId, BondAttachment attachment, List<Bond> previousBonds, BondType newBondType)
        {
            bool result = true;
            if (!attachment.IsGuide() && bonds.ContainsKey(instanceId))
            {
                Bond cb = null;
                foreach(Bond b in bonds[instanceId])
                {
                    cb = b.GetCollidingBond(attachment);
                    if (cb != null)
                    {
                        break;
                    }
                }

                if (cb != null)
                {
                    bool oldIsFlow = cb.ChainType != ChainType.None;
                    bool newIsLock = newBondType == BondType.Lock;
                    // locks can't break flow bonds
                    if (oldIsFlow && newIsLock)
                    {
                        result = false;
                    }
                    else
                    {
                        RemoveBond(cb);
                        previousBonds.Add(cb);
                    }
                }
            }
            return result;
        }


        public List<Bond> GetBondsForInstances(uint[] instanceIds)
        {
            List<Bond> result = new List<Bond>();
            foreach (uint id in instanceIds)
            {
                if (bonds.ContainsKey(id))
                {
                    result.AddRange(bonds[id]);
                }
            }
            return result;
        }
        public void GetBondsForInstance(uint instanceId, List<Bond> bondList)
        {
            if (bonds.ContainsKey(instanceId))
            {
                bondList.AddRange(bonds[instanceId]);
            }
        }
        public Bond GetGuideBond(uint guideId)
        {
            Bond result = null;
            if (bonds.ContainsKey(guideId))
            {
                result = bonds[guideId][0];
            }
            return result;
        }
        public void GetBondsForGuide(uint guideId, List<uint> guideIds)
        {
            if (guideAttachments.ContainsKey(guideId))
            {
                foreach (Bond b in guideAttachments[guideId])
                {
                    guideIds.Add(b.SourceInstanceId);
                }
            }
        }
        private List<Bond> GetExternalBondIds(ICollection<uint> alreadyDiscovered, uint id, HashSet<Bond> starts)
        {
            List<Bond> result = new List<Bond>();

            GetBondsForInstance(id, result);
            if (result.Count > 0)
            {
                List<Bond> localStarts = new List<Bond>();
                foreach (Bond b in result)
                {
                    if (b.ChainType != ChainType.None)
                    {
                        Bond start = b.GetFirst();
                        localStarts.Add(start);
                        starts.Add(start);
                    }
                }

                foreach (Bond start in localStarts)
                {
                    List<Bond> chain = new List<Bond>();
                    Bond b = start;
                    do
                    {
                        chain.Add(b);
                        if (!alreadyDiscovered.Contains(b.SourceInstanceId))// && !b.SourceAttachment.IsGuide())
                        {
                            result.Add(b);
                        }
                        b = b.Next;
                    }
                    while (b != null && !b.IsStart);
                }
            }
            return result;
        }
        public void ConstrainOffset(IEnumerable<uint> sourceIds, ref Vex.Point offset)
        {
            HashSet<uint> tested = new HashSet<uint>();
            HashSet<uint> alreadyDiscovered = new HashSet<uint>();
            List<Bond> guideBonds = new List<Bond>();
            AppendJoinedRelations(sourceIds, tested, alreadyDiscovered, guideBonds);
            foreach (Bond b in guideBonds)
            {
                offset.X = b.Next.SourceAttachment.IsVGuide() ? 0 : offset.X;
                offset.Y = b.Next.SourceAttachment.IsHGuide() ? 0 : offset.Y;
            }
        }
        public void ConstrainBounds(IEnumerable<uint> sourceIds, ref Vex.Rectangle bounds, ChainType chainType)
        {
            HashSet<uint> tested = new HashSet<uint>();
            HashSet<uint> alreadyDiscovered = new HashSet<uint>();
            List<Bond> guideBonds = new List<Bond>();
            AppendJoinedRelations(sourceIds, tested, alreadyDiscovered, guideBonds);
            foreach (Bond b in guideBonds)
            {
                if (!chainType.IsHorizontal() && b.Next.SourceAttachment.IsVGuide())
                {
                    bounds = bounds.Intersect(MainForm.CurrentInstanceManager[b.SourceInstanceId].StrokeBounds);
                }
                else if (chainType.IsHorizontal() && b.Next.SourceAttachment.IsHGuide())
                {
                    bounds = bounds.Intersect(MainForm.CurrentInstanceManager[b.SourceInstanceId].StrokeBounds);
                }
            }
        }
        public void AppendJoinedRelations(IEnumerable<uint> testingIds, HashSet<uint> result, ICollection<uint> alreadyDiscovered, List<Bond> guideBonds)
        {
            List<uint> newIds = new List<uint>();

            foreach (uint id in testingIds)
            {
                result.Add(id);
                if (bonds.ContainsKey(id))
                {
                    foreach (Bond b in bonds[id])
                    {
                        if (b.BondType == BondType.Join)
                        {
                            if (b.Next != null &&
                                result.Add(b.Next.SourceInstanceId) &&
                                !b.Next.SourceAttachment.IsGuide() &&
                                !alreadyDiscovered.Contains(b.Next.SourceInstanceId))
                            {
                                newIds.Add(b.Next.SourceInstanceId);
                            }

                            if (b.Previous != null &&
                                result.Add(b.Previous.SourceInstanceId) &&
                                !b.Previous.SourceAttachment.IsGuide() &&
                                !alreadyDiscovered.Contains(b.Previous.SourceInstanceId))
                            {
                                newIds.Add(b.Previous.SourceInstanceId);
                            }
                        }
                        else if (b.BondType == BondType.Lock)
                        {
                            guideBonds.Add(b);
                        }
                    }
                }
            }

            if (newIds.Count > 0)
            {
                AppendJoinedRelations(newIds, result, alreadyDiscovered, guideBonds);
            }
        }
        public BondType[] GetHandlesForInstance(uint instanceId)
        {
            BondType[] result;
            if (handles.ContainsKey(instanceId))
            {
                result = handles[instanceId];
            }
            else
            {
                result = emptyHandles;
            }
            return result;
        }
        public void SortBondChain(IEnumerable<uint> selected, Bond start, Dictionary<uint, Vex.Rectangle> transforms)
        {
            List<Bond> bondChain = new List<Bond>() { };
            Bond first = start;
            Bond last = start;

            while (first != null)
            {
                bondChain.Add(first);
                first.TargetLocation = transforms[first.SourceInstanceId].Point;
                last = first;
                first = first.Next;
            }

            if (start.ChainType == ChainType.DistributedHorizontal)
            {
                DistributeHorizontally(bondChain, transforms);
            }
            else if (start.ChainType == ChainType.DistributedVertical)
            {
                DistributeVertically(bondChain, transforms);
            }            
            
            bool isOuterDist = start.ChainType.IsDistributed() && 
                (selected.Contains(start.SourceInstanceId) || selected.Contains(last.SourceInstanceId));

            if (!isOuterDist && bondChain.Count > 1)
            {
                bondChain.Sort();

                bondChain[0].Previous = null;
                for (int i = 1; i < bondChain.Count; i++)
                {
                    bondChain[i - 1].Next = bondChain[i];
                    bondChain[i].Previous = bondChain[i - 1];
                }
                bondChain[bondChain.Count - 1].Next = null;

                //if (start.ChainType == ChainType.DistributedHorizontal)
                //{
                //    DistributeHorizontally(bondChain, transforms);
                //}
                //else if (start.ChainType == ChainType.DistributedVertical)
                //{
                //    DistributeVertically(bondChain, transforms);
                //}
            }

        }

        private void DistributeHorizontally(List<Bond> bondChain, Dictionary<uint, Vex.Rectangle> transforms)
        {
            Vex.Rectangle diFirst = transforms[bondChain[0].SourceInstanceId];
            Vex.Rectangle diLast = transforms[bondChain[bondChain.Count - 1].SourceInstanceId];
            float firstWidth = diFirst.Width;
            float lastWidth = diLast.Width;
            float fillWidth = diLast.Left - (diFirst.Left + diFirst.Width);
            float remainingWidths = 0;

            for (int i = 1; i < bondChain.Count - 1; i++)
            {
                DesignInstance di = MainForm.CurrentInstanceManager[bondChain[i].SourceInstanceId];
                remainingWidths += di.StrokeBounds.Width;
            }

            float spacing = (fillWidth - remainingWidths) / (bondChain.Count - 1);

            float curLoc = diFirst.Left;
            for (int i = 0; i < bondChain.Count - 1; i++)
            {
                uint id = bondChain[i].SourceInstanceId;
                Vex.Rectangle r = transforms[id];

                transforms[id] = new Vex.Rectangle(curLoc, r.Top, r.Width, r.Height);

                curLoc += spacing + r.Width;
            }
        }
        private void DistributeVertically(List<Bond> bondChain, Dictionary<uint, Vex.Rectangle> transforms)
        {
            Vex.Rectangle diFirst = transforms[bondChain[0].SourceInstanceId];
            Vex.Rectangle diLast = transforms[bondChain[bondChain.Count - 1].SourceInstanceId];
            float firstHeight = diFirst.Height;
            float lastHeight = diLast.Height;
            float fillHeight = diLast.Top - (diFirst.Top + diFirst.Height);
            float remainingHeights = 0;

            for (int i = 1; i < bondChain.Count - 1; i++)
            {
                DesignInstance di = MainForm.CurrentInstanceManager[bondChain[i].SourceInstanceId];
                remainingHeights += di.StrokeBounds.Height;
            }

            float spacing = (fillHeight - remainingHeights) / (bondChain.Count - 1);

            float curLoc = diFirst.Top;
            for (int i = 0; i < bondChain.Count - 1; i++)
            {
                uint id = bondChain[i].SourceInstanceId;
                Vex.Rectangle r = transforms[id];

                transforms[id] = new Vex.Rectangle(r.Left, curLoc, r.Width, r.Height);

                curLoc += spacing + r.Height;
            }
        }

        public void Align(uint[] instanceIds, ChainType chainType, Vex.Point target, List<Bond> addedBonds, List<Bond> previousBonds)
        {
            BondAttachment bt = chainType.GetAttachment();
            BondType bondType = chainType.IsAligned() ? BondType.Anchor : BondType.Spring;

            Bond b;
            Bond prev = null;
            for (int i = 0; i < instanceIds.Length - 1; i++)
            {
                uint id = instanceIds[i];

                if (chainType.IsDistributed())
                {
                    if (i > 0)
                    {
                        RemoveCollidingBond(id, bt, previousBonds, BondType.Anchor);
                    }
                    if (i < instanceIds.Length - 1)
                    {
                        BondAttachment btOpp = chainType.GetOppositeAttachment();
                        RemoveCollidingBond(id, btOpp, previousBonds, BondType.Anchor);
                    }
                }
                else
                {
                    RemoveCollidingBond(id, bt, previousBonds, BondType.Anchor);
                }

                b = new Bond(id, bt, bondType);
                b.ChainType = chainType;
                b.TargetLocation = target;
                AddBond(b);
                addedBonds.Add(b);

                b.Previous = prev;
                if (prev != null)
                {
                    prev.Next = b;
                }
                prev = b;
            }

            int lastIndex = instanceIds.Length - 1;
            uint lastId = instanceIds[lastIndex];
            RemoveCollidingBond(lastId, bt, previousBonds, BondType.Anchor);

            b = new Bond(lastId, bt, bondType);
            b.ChainType = chainType;
            b.TargetLocation = target;
            AddBond(b);
            addedBonds.Add(b);

            b.Previous = prev;
            prev.Next = b;
        }

        public void JoinHandles(
            uint srcId, BondAttachment srcHandle, 
            uint targId, BondAttachment targHandle,
            Vex.Point targetLocation, List<Bond> addedBonds, List<Bond> previousBonds)
        {
            RemoveCollidingBond(srcId, srcHandle, previousBonds, BondType.Join);
            RemoveCollidingBond(targId, targHandle, previousBonds, BondType.Join);

            Bond src = new Bond(srcId, srcHandle, BondType.Join);
            Bond targ = new Bond(targId, targHandle, BondType.Join);
            AddBond(src);
            AddBond(targ);

            src.Next = targ;
            targ.Previous = src;

            src.TargetLocation = targetLocation;
            targ.TargetLocation = targetLocation;

            addedBonds.Add(src);
            addedBonds.Add(targ);
        }

        public void LockToGuide(
            uint srcId, BondAttachment srcHandle, 
            uint targId, BondAttachment targHandle,
            Vex.Point targetLocation, List<Bond> addedBonds, List<Bond> previousBonds)
        {
            bool canAdd = RemoveCollidingBond(srcId, srcHandle, previousBonds, BondType.Lock);
            if (canAdd)
            {
                Bond src = new Bond(srcId, srcHandle, BondType.Lock);
                Bond targ = GetOrCreateGuideBond(targId, targHandle);

                src.TargetLocation = targetLocation;
                targ.TargetLocation = targetLocation;

                AddBond(src);
                src.Next = targ;

                addedBonds.Add(src);
                guideAttachments[targId].Add(src);
            }
        }

        public void PinToPaper(uint[] instanceIds)
        {
        }

        public void AnchorObjects(uint instanceIdA, BondAttachment snapTargetA, uint instanceIdB, BondAttachment snapTargetB)
        {
        }


        public void GetRelatedObjects(IEnumerable<uint> ids, HashSet<uint> alreadyDiscovered)
        {
            List<Bond> externalBonds = new List<Bond>();
            DesignTimeline designStage = MainForm.CurrentStage.CurrentEditItem;

            alreadyDiscovered.UnionWith(ids);
            
            List<uint> newlyDiscovered = new List<uint>();
            foreach (uint id in ids)
            {
                if (bonds.ContainsKey(id))
                {
                    foreach(Bond b in bonds[id])
                    {
                        if(b.Next != null && !b.Next.SourceAttachment.IsGuide() && alreadyDiscovered.Add(b.Next.SourceInstanceId))
                        {
                            newlyDiscovered.Add(b.Next.SourceInstanceId);
                        }

                        if (b.Previous != null && !b.Previous.SourceAttachment.IsGuide() && alreadyDiscovered.Add(b.Previous.SourceInstanceId))
                        {
                            newlyDiscovered.Add(b.Previous.SourceInstanceId);
                        }
                    }
                }                
            }

            if(newlyDiscovered.Count > 0)
            {
                GetRelatedObjects(newlyDiscovered, alreadyDiscovered);
            }
        }
        
        public void GetRelatedObjectTransforms(IEnumerable<uint> ids, Dictionary<uint, Vex.Rectangle> alreadyDiscovered, Vex.Point offset)
        {
            HashSet<Bond> starts = new HashSet<Bond>();
            GetRelatedObjectTransformsRec(ids, alreadyDiscovered, offset, starts);

            foreach (Bond start in starts)
            {
                SortBondChain(ids, start, alreadyDiscovered);
            }

        }
        private void GetRelatedObjectTransformsRec(IEnumerable<uint> ids, Dictionary<uint, Vex.Rectangle> alreadyDiscovered, Vex.Point offset, HashSet<Bond> starts)
        {
            List<Bond> externalBonds = new List<Bond>();
            DesignTimeline designStage = MainForm.CurrentStage.CurrentEditItem;

            //alreadyDiscovered.UnionWith(ids);
            //foreach (uint id in ids)
            //{
            //    alreadyDiscovered.Add(id, designStage[id].StrokeBounds.TranslatedRectangle(tx, ty));
            //}

            List<Bond> guideBonds = new List<Bond>();
            HashSet<uint> joinedIds = new HashSet<uint>();
            designStage.BondStore.AppendJoinedRelations(ids, joinedIds, alreadyDiscovered.Keys, guideBonds);
            foreach (Bond b in guideBonds)
            {
                if (b.Next.SourceAttachment.IsGuide())
                {
                    bool guideMoved = b.Next.GuideMoved;
                    Vex.Point guideOffset = b.Next.GuideMoved ? offset : Vex.Point.Zero;
                    offset.X = b.Next.SourceAttachment.IsVGuide() ? guideOffset.X : offset.X;
                    offset.Y = b.Next.SourceAttachment.IsHGuide() ? guideOffset.Y : offset.Y;   
                }      
            }

            foreach (uint id in joinedIds)
            {
                if (!alreadyDiscovered.ContainsKey(id))
                {
                    alreadyDiscovered.Add(id, designStage[id].StrokeBounds.TranslatedRectangle(offset.X, offset.Y));
                }
                externalBonds.AddRange(designStage.BondStore.GetExternalBondIds(alreadyDiscovered.Keys, id, starts));
            }

            foreach (Bond b in externalBonds)
            {
                // todo: this algorithm ignores multiple chains on an object.
                // probably need to keep track of 'join islands' and their outward chains
                if (!alreadyDiscovered.ContainsKey(b.SourceInstanceId))
                {
                    float newTx = offset.X;
                    float newTy = offset.Y;
                    if (offset.X != 0 || offset.Y != 0)
                    {
                        designStage.BondStore.GetFinalOffset(b, ref newTx, ref newTy);
                    }
                    GetRelatedObjectTransformsRec(new uint[] { b.SourceInstanceId }, alreadyDiscovered, new Vex.Point(newTx, newTy), starts);
                }
            }
        }

        public void GetFinalOffset(Bond b, ref float tx, ref float ty)
        {
            //Bond[] bonds = GetBondsForInstance(instId);

            if (b.ChainType == ChainType.None)
            {
            }
            else if (b.ChainType.IsDistributed())
            {
            }
            else if (b.ChainType.IsHorizontal())
            {
                // this clamps the motion for the non selected elements
                // a top aligned object can still move up and down if a bonded partner is moving up and down
                // but doesn't get any side to side push
                tx = 0;
            }
            else
            {
                ty = 0;
            }
        }


        public static BondType[] emptyHandles = new BondType[]
        {
            BondType.Handle, // TL
            BondType.Handle, // T
            BondType.Handle, // TR
            BondType.Handle, // R
            BondType.Handle, // BR
            BondType.Handle, // B
            BondType.Handle, // BL
            BondType.Handle, // L
            BondType.Handle, // C
            BondType.Handle  // RP
        };
    }
}
