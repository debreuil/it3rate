using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Display;
using DDW.Views;

namespace DDW.Commands
{
    class TranslateRotationCenterCommand : ICommand
    {
        Vex.Point newLocation;
        Vex.Point prevLocation;

        public TranslateRotationCenterCommand(Vex.Point prevLocation, Vex.Point newLocation)
        {
            this.prevLocation = prevLocation;
            this.newLocation = newLocation;
        }

        public void Execute()
        {
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            sel.GlobalRotationCenter = newLocation;
            MainForm.CurrentStage.InvalidateCenterPoint();
        }

        public void UnExecute()
        {
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            sel.GlobalRotationCenter = prevLocation;
            MainForm.CurrentStage.InvalidateCenterPoint();
        }
    }
}