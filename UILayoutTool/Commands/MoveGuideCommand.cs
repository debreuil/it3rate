using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Vex.Bonds;

namespace DDW.Commands
{
    public class MoveGuideCommand : ICommand, ISaveableCommand
    {
        Guide guide;
        int offsetX;
        int offsetY;
        List<Bond> addedBonds = new List<Bond>();
        List<Bond> previousBonds = new List<Bond>();

        public MoveGuideCommand(Guide guide, int offsetX, int offsetY)
        {
            this.guide = guide;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            stage.MoveGuide(guide, offsetX, offsetY, addedBonds, previousBonds);
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

            stage.MoveGuide(guide, -offsetX, -offsetY, addedBonds, previousBonds);

            foreach (Bond b in previousBonds)
            {
                stage.CurrentEditItem.BondStore.AddBond(b);
            }
        }
    }
}
