using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex.Bonds;

namespace DDW.Data
{
    public struct CrossRange : ICrossPoint
    {
        private float crossStart;
        private uint instanceHash;
        private BondAttachment bondAttachment;
        public float CrossStart { get { return crossStart; } }
        public float crossEnd;
        public float CrossEnd { get { return crossEnd; } }
        public uint InstanceHash { get { return instanceHash; } }
        public BondAttachment BondAttachment { get { return bondAttachment; } }

        public CrossRange(float start, float end, uint instanceSource, BondAttachment bondAttachment)
        {
            this.crossStart = start;
            this.crossEnd = end;
            this.instanceHash = instanceSource;
            this.bondAttachment = bondAttachment;
        }
    }
}
