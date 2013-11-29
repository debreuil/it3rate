using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Enums;
using DDW.Views;
using DDW.Display;

namespace DDW.Commands
{
    public class RotateTransformCommand : ICommand, IRepeatableCommand, ISaveableCommand
    {
        float angle;
        Vex.Point rotateCenter;

        public RotateTransformCommand(float angle)
        {
            this.angle = angle;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            this.rotateCenter = stage.Selection.GlobalRotationCenter;
            stage.RotateSelectionAt(angle, rotateCenter);
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.RotateSelectionAt(-angle, rotateCenter);
        }

        public IRepeatableCommand GetRepeatCommand()
        {
            return new RotateTransformCommand(angle);
        }
    }
}
