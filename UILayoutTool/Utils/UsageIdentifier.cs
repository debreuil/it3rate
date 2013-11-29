using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;

namespace DDW.Utils
{
    public class UsageIdentifier
    {
        public DesignTimeline Parent;
        public DesignInstance Instance;
        public int Depth;

        public UsageIdentifier(DesignTimeline parent, DesignInstance instance, int depth)
        {
            Parent = parent;
            Instance = instance;
            Depth = depth;
        }
    }
}
