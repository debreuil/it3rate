
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using Vex = DDW.Vex;
using DDW.Views;
using DDW.Vex.Bonds;

namespace DDW.Commands
{
    public class RemoveGuideCommand : ICommand, ISaveableCommand
    {
        Guide guide;

        public RemoveGuideCommand(Guide guide)
        {
            this.guide = guide;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.RemoveGuide(guide);
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.AddGuide(guide);
        }
    }
}
