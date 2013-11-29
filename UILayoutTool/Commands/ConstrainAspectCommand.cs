using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Views;
using Vex = DDW.Vex;
using DDW.Display;
using DDW.Interfaces;

namespace DDW.Commands
{
    public class ConstrainAspectCommand : ICommand, ISaveableCommand
    {
        private Vex.AspectConstraint prevConstraint;
        private Vex.AspectConstraint constraint;

        public ConstrainAspectCommand(Vex.AspectConstraint constraint)
        {
            this.constraint = constraint;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;

            uint[] selIds = stage.Selection.SelectedIds;
            foreach (uint id in selIds)
	        {
                DesignInstance di = stage.InstanceManager[id];
                prevConstraint = di.AspectConstraint;
                di.AspectConstraint = constraint;
	        }

            stage.Selection.Update();
            MainForm.CurrentStage.InvalidateSelection();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            
            uint[] selIds = stage.Selection.SelectedIds;
            foreach (uint id in selIds)
	        {
                DesignInstance di = stage.InstanceManager[id];
                di.AspectConstraint = prevConstraint;
	        }

            stage.Selection.Update();
            MainForm.CurrentStage.InvalidateSelection();
        }
    }
}
