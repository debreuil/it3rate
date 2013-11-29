using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Views;
using DDW.Display;
using DDW.Managers;
using DDW.Interfaces;
using DDW.Utils;

namespace DDW.Commands
{
    class RemoveTransformsCommand : ICommand, ISaveableCommand
    {
        uint[] instanceIds;
        Vex.Matrix[] prevMatrices;
        Vex.Matrix prevTransforMatrix;

        public RemoveTransformsCommand(uint[] instanceIds)
        {
            this.instanceIds = instanceIds;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;

            prevTransforMatrix = stage.Selection.TransformMatrix.VexMatrix();
            prevMatrices = stage.RemoveSelectionTransform();             
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.ReaddSelectionTransform(prevTransforMatrix, prevMatrices);
        }
    }
}
