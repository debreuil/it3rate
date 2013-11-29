using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using System.Drawing;
using DDW.Views;
using Vex = DDW.Vex;

namespace DDW.Commands
{
    class SwapDepthsCommand : ICommand, ISaveableCommand
    {
        int[] from;
        int[] to;

        public SwapDepthsCommand(int[] from, int[] to)
        {
            this.from = from;
            this.to = to;
        }

        public void Execute()
        {
            DDW.Display.DesignTimeline dl = MainForm.CurrentStage.CurrentEditItem;
            dl.SwapDepths(from, to);
        }

        public void UnExecute()
        {
            DDW.Display.DesignTimeline dl = MainForm.CurrentStage.CurrentEditItem;

            int[] revFrom = (int[])from.Clone();
            Array.Reverse(revFrom);
            int[] revTo = (int[])to.Clone();
            Array.Reverse(revTo);

            dl.SwapDepths(revTo, revFrom);
        }
    }
}
