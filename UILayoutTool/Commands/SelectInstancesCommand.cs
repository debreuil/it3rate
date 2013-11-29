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
    public class SelectInstancesCommand : ICommand
    {
        uint[] instanceIds;
        SelectionModifier modifier;
        uint[] prevSelected;

        public SelectInstancesCommand(uint[] instanceIds, SelectionModifier modifier)
        {
            this.instanceIds = instanceIds;
            this.modifier = modifier;
        }

        public void Execute()
        {
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            prevSelected = sel.SelectedIds;
            if (modifier == SelectionModifier.AddToSelection)
            {
                uint[] allSelected = prevSelected.Union(instanceIds).ToArray<uint>();
                sel.Set(allSelected);
            }
            else if(modifier == SelectionModifier.SubtractFromSelection)
            {
                uint[] allSelected = prevSelected.Except(instanceIds).ToArray<uint>();
                sel.Set(allSelected);
            }
            else
            {
                sel.Set(instanceIds);
                MainForm.CurrentStage.ResetTransformHandles();
            }
        }

        public void UnExecute()
        {
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            sel.Set(prevSelected);
            MainForm.CurrentStage.ResetTransformHandles();
        }
    }

    public enum SelectionModifier
    {
        AddToSelection,
        SubtractFromSelection,
        SetSelection,
    }

}
