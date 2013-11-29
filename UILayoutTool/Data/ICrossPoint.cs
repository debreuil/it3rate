using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex.Bonds;

namespace DDW.Data
{
    public interface ICrossPoint
    {
        float CrossStart { get; }
        float CrossEnd { get; }
        uint InstanceHash { get; }
        BondAttachment BondAttachment { get; }
    }
}
