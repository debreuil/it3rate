using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Display;
using DDW.Managers;
using System.Drawing.Drawing2D;
using DDW.Views;

namespace DDW.Commands
{
    class EditInPlaceCommand : ICommand
    {
        uint[] instanceId;
        bool isBegin;
        int popCount;

        public EditInPlaceCommand(uint instanceId)
        {
            this.instanceId = new uint[]{instanceId};
            this.isBegin = true;
        }
        public EditInPlaceCommand(int popCount)
        {
            this.isBegin = false;
            this.popCount = popCount;
        }

        public void Execute()
        {
            StageView sv = MainForm.CurrentStage;
            if (isBegin)
            {
                sv.EditInPlacePush(instanceId[0]);
                popCount = 1;

                //for (int i = 0; i < instanceId.Length; i++)
                //{
                //    sv.EditInPlacePush(instanceId[i]);                    
                //}
                //popCount = instanceId.Length;
            }
            else
            {            
                instanceId = sv.EditInPlacePop(popCount); 
            }
        }

        public void UnExecute()
        {
            StageView sv = MainForm.CurrentStage;
            if (isBegin)
            {
                sv.EditInPlacePop(popCount);
            }
            else
            {
                // can push into multiple levels at once
                // when undoing a bread crumbs click
                for (int i = instanceId.Length - 1; i >= 0; i--)
                {
                    sv.EditInPlacePush(instanceId[i]);
                }
            }
        }
    }
}
