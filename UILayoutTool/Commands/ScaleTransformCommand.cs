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
    public class ScaleTransformCommand : ICommand, IRepeatableCommand, ISaveableCommand
    {
        float scaleX;
        float scaleY;
        Vex.Point scaleCenter;

        Vex.Point prevLocation;

        public ScaleTransformCommand(float scaleX, float scaleY, Vex.Point scaleCenter)
        {
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleCenter = scaleCenter;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            prevLocation = stage.Selection.StrokeBounds.Point;
            stage.ScaleSelectionAt(scaleX, scaleY, scaleCenter);
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.ScaleSelectionAt(1f / scaleX, 1f / scaleY, scaleCenter);
        }

        public IRepeatableCommand GetRepeatCommand()
        {
            return new ScaleTransformCommand(scaleX, scaleY, scaleCenter);
        }
    }
}
