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
    public class DeleteInstancesCommand : ICommand, ISaveableCommand
    {
        uint[] instanceIds;
        uint[] prevSelected;
        RemoveBondsCommand removeBondsCommand;

        public DeleteInstancesCommand(uint[] instanceIds)
        {
            this.instanceIds = instanceIds;
            removeBondsCommand = new RemoveBondsCommand();
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            removeBondsCommand.Execute();
            prevSelected = sel.SelectedIds;
            stage.RemoveInstancesById(instanceIds);
            MainForm.CurrentStage.InvalidateTransformedSelection();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            InstanceGroup sel = MainForm.CurrentStage.Selection;
            stage.AddInstancesById(instanceIds);
            removeBondsCommand.UnExecute();
            sel.Set(prevSelected);
            MainForm.CurrentStage.ResetTransformHandles();
            MainForm.CurrentStage.InvalidateTransformedSelection();
        }
    }
}
