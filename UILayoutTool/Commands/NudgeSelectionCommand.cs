using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Vex.Bonds;

namespace DDW.Commands
{
    public class NudgeSelectionCommand : ICommand, IRepeatableCommand, ISaveableCommand
    {
        Vex.Point offset;
        List<Bond> addedBonds = new List<Bond>();
        List<Bond> previousBonds = new List<Bond>();

        public NudgeSelectionCommand(float xAmount, float yAmount)
        {
            offset = new Vex.Point(xAmount, yAmount);
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.TranslateSelection(offset, false, addedBonds, previousBonds);
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

            stage.TranslateSelection(new Vex.Point(-offset.X, -offset.Y), false, discardAddedBonds, discardPreviousBonds);

            foreach (Bond b in previousBonds)
            {
                stage.CurrentEditItem.BondStore.AddBond(b);
            }
        }

        public void AppendNudge(int xAmount, int yAmount)
        {
            StageView stage = MainForm.CurrentStage;
            stage.TranslateSelection(new Vex.Point(xAmount, yAmount), false, addedBonds, previousBonds);

            offset = new Vex.Point(offset.X + xAmount, offset.Y + yAmount);
        }

        public IRepeatableCommand GetRepeatCommand()
        {
            return new NudgeSelectionCommand(offset.X, offset.Y);
        }
    }
}
