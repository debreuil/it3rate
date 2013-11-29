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
    public class SelectionToSymbolCommand : ICommand, ISaveableCommand
    {
        uint[] prevSelected;
        uint newLibraryItemId;
        uint newInstanceId;
        Vex.Point prevOffset;

        public SelectionToSymbolCommand()
        {
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;

            // store old selection
            prevSelected = stage.Selection.IdsByDepth;// SelectedIds;
            prevOffset = stage.Selection.StrokeBounds.Point;


            // create symbol from selected
            Vex.Timeline tl = new Vex.Timeline(stage.Library.NextLibraryId());

            tl.Name = stage.Library.GetNextDefaultName();
            tl.StrokeBounds = stage.Selection.StrokeBounds.TranslatedRectangle(-prevOffset.X, -prevOffset.Y);

            // delete old symbols
            DesignInstance[] oldInstances = stage.RemoveInstancesById(prevSelected);

            for (int i = 0; i < prevSelected.Length; i++)
			{
                DesignInstance inst = oldInstances[i];// instMgr[prevSelected[i]];
                Vex.Matrix m = inst.GetMatrix();
                m.TranslateX -= prevOffset.X;
                m.TranslateY -= prevOffset.Y;
                inst.SetMatrix(m);
                tl.AddInstance(inst.Instance);
                stage.InstanceManager.AddInstance(inst); // reusing, so must readd (todo: don't  reuse ids?)
			}

            LibraryItem li = stage.CreateLibraryItem(tl, true);
            newLibraryItemId = li.DefinitionId;
            stage.Library.AddLibraryItem(li);
            LibraryView.CurrentLibraryView.AddItemToLibrary(li);

            // add new symbol to stage
            DesignInstance di = stage.AddInstance(tl.Id, prevOffset);
            newInstanceId = di.InstanceHash;

            // select new symbol
            stage.Selection.Set(new uint[] { di.InstanceHash });
            stage.ResetTransformHandles();
            stage.InvalidateSelection();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;

            stage.Selection.Clear();
            stage.RemoveInstancesById(new uint[] { newInstanceId });
            LibraryView.CurrentLibraryView.RemoveItemFromLibrary(newLibraryItemId);

            stage.AddInstancesById(prevSelected);

            for (int i = 0; i < prevSelected.Length; i++)
            {
                DesignInstance inst = stage.InstanceManager[prevSelected[i]];
                Vex.Matrix m = inst.GetMatrix();
                m.TranslateX += prevOffset.X;
                m.TranslateY += prevOffset.Y;
                stage.SetDesignInstanceMatrix(inst, m);
            }

            stage.Selection.Set(prevSelected);
            stage.ResetTransformHandles();
        }
    }
}
