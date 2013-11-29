using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using Vex = DDW.Vex;
using DDW.Views;
using DDW.Vex.Bonds;

namespace DDW.Commands
{
    public class RemoveBondsCommand : ICommand, ISaveableCommand
    {
        Bond[] removedBonds; 

        public RemoveBondsCommand()
        {
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            removedBonds = stage.CurrentEditItem.BondStore.GetBondsForInstances(stage.Selection.SelectedIds).ToArray();
            stage.CurrentEditItem.BondStore.RemoveBonds(removedBonds);
            stage.Selection.Update();
            MainForm.CurrentStage.InvalidateSelection();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            foreach (Bond b in removedBonds)
            {
                stage.CurrentEditItem.BondStore.AddBond(b);
            }
            stage.Selection.Update();
            MainForm.CurrentStage.InvalidateSelection();
        }
    }
}
