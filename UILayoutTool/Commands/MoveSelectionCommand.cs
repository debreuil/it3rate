using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using System.Drawing;
using DDW.Views;
using Vex = DDW.Vex;
using DDW.Display;
using DDW.Vex.Bonds;

namespace DDW.Commands
{
    class MoveSelectionCommand : ICommand, IRepeatableCommand, ISaveableCommand
    {
        Vex.Point offset;
        List<Bond> addedBonds = new List<Bond>();
        List<Bond> previousBonds = new List<Bond>();
        bool useSmartBonds;

        public MoveSelectionCommand(Vex.Point offset)
        {
            this.offset = offset;
            useSmartBonds = MainForm.CurrentStage.UseSmartBonds;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.TranslateSelection(offset, useSmartBonds, addedBonds, previousBonds);
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            List<Bond> discardAddedBonds = new List<Bond>();
            List<Bond> discardPreviousBonds = new List<Bond>();

            foreach (Bond b in addedBonds)
            {
                stage.CurrentEditItem.BondStore.RemoveBond(b);
            }

            Vex.Point unOffset = new Vex.Point(-offset.X, -offset.Y);
            stage.TranslateSelection(unOffset, false, discardAddedBonds, discardPreviousBonds);

            foreach (Bond b in previousBonds)
            {
                stage.CurrentEditItem.BondStore.AddBond(b);
            }

            addedBonds.Clear();
            previousBonds.Clear();
        }

        public IRepeatableCommand GetRepeatCommand()
        {
            return new MoveSelectionCommand(offset);
        }
    }
}
