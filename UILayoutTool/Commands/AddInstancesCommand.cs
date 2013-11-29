using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DDW.Display;
using DDW.Managers;
using DDW.Views;
using DDW.Interfaces;
using Vex = DDW.Vex;

namespace DDW.Commands
{
    public class AddInstancesCommand : ICommand, ISaveableCommand
    {
        uint[] libraryIds;
        uint[] instanceIds;
        Vex.Point[] locations;
        uint[] prevSelected;

        public AddInstancesCommand(uint[] libraryIds, Vex.Point[] locations)
        {
            this.libraryIds = libraryIds;
            this.locations = locations;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            InstanceGroup sel = stage.Selection;
            prevSelected = sel.SelectedIds;
            if (instanceIds == null)
            {
                instanceIds = stage.AddInstances(libraryIds, locations);
            }
            else
            {
                stage.AddInstancesById(instanceIds);
            }

            sel.Set(instanceIds);
            stage.ResetTransformHandles();
            stage.InvalidateTransformedSelection();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;

            InstanceGroup sel = stage.Selection;
            stage.RemoveInstancesById(instanceIds);
            sel.Set(prevSelected);
            stage.ResetTransformHandles();
            stage.InvalidateTransformedSelection();
        }

    }
}
