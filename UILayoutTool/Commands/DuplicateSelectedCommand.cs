using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Display;
using DDW.Managers;

namespace DDW.Commands
{
    public class DuplicateSelectedCommand : ICommand, IRepeatableCommand, ISaveableCommand
    {
        uint[] newInstanceIds;
        Vex.Point offset;
        uint[] prevSelected;

        public DuplicateSelectedCommand(Vex.Point offset)
        {
            this.offset = offset;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;

            InstanceGroup sel = stage.Selection;
            this.prevSelected = sel.SelectedIds;

            Vex.Point selRotCent = sel.GlobalRotationCenter.Translate(sel.Location.Negate());

            uint[] libraryIds = new uint[prevSelected.Length];
            Vex.Point[] locations = new Vex.Point[prevSelected.Length];
            for (int i = 0; i < prevSelected.Length; i++)
            {
                DesignInstance di = MainForm.CurrentInstanceManager[prevSelected[i]];
                libraryIds[i] = di.LibraryItem.DefinitionId;
                locations[i] = new Vex.Point(di.Location.X + offset.X, di.Location.Y + offset.Y);
            }

            if (newInstanceIds == null)
            {
                newInstanceIds = stage.AddInstances(libraryIds, locations);
                for (int i = 0; i < newInstanceIds.Length; i++)
                {
                    DesignInstance oldDi = MainForm.CurrentInstanceManager[prevSelected[i]];
                    DesignInstance newDi = MainForm.CurrentInstanceManager[newInstanceIds[i]];
                    Vex.Matrix m = oldDi.GetMatrix();
                    stage.SetDesignInstanceMatrix(newDi, new Vex.Matrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, newDi.Location.X, newDi.Location.Y));

                    newDi.RotationCenter = oldDi.RotationCenter;
                }
            }
            else
            {
                stage.AddInstancesById(newInstanceIds);
            }

            sel.Set(newInstanceIds);
            sel.GlobalRotationCenter = selRotCent.Translate(sel.Location);
            stage.ResetTransformHandles();
            stage.InvalidateTransformedSelection();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;

            InstanceGroup sel = stage.Selection;
            stage.RemoveInstancesById(newInstanceIds);
            sel.Set(prevSelected);
            stage.ResetTransformHandles();
            stage.InvalidateTransformedSelection();
        }

        public IRepeatableCommand GetRepeatCommand()
        {
            return new DuplicateSelectedCommand(offset);
        }
    }
}
