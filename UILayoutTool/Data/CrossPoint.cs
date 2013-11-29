using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex.Bonds;

namespace DDW.Data
{
    public struct CrossPoint : ICrossPoint
    {
        private float crossStart;
        private uint instanceHash;
        private BondAttachment bondAttachment;
        public float CrossStart { get { return crossStart; } }
        public float CrossEnd { get { return float.PositiveInfinity; } }
        public uint InstanceHash { get { return instanceHash; } }
        public BondAttachment BondAttachment { get { return bondAttachment; } }

        public CrossPoint(float start, uint instanceSource, BondAttachment bondAttachment)
        {
            this.crossStart = start;
            this.instanceHash = instanceSource;
            this.bondAttachment = bondAttachment;
        }
    }
}
