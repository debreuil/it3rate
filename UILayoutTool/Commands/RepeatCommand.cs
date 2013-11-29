using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;

namespace DDW.Commands
{
    class RepeatCommand : ICommand, IRepeatableCommand, ISaveableCommand
    {
        IRepeatableCommand[] commands;

        public RepeatCommand()
        {
        }

        public void Execute()
        {
            CommandStack cs = MainForm.CurrentStage.CommandStack;

            if (commands == null)
            {
                commands = cs.GetRepeatables();
            }

            for (int i = 0; i < commands.Length; i++)
            {
                ((ICommand)commands[i]).Execute();
            }
        }

        public void UnExecute()
        {
            for (int i = commands.Length - 1; i >= 0; i--)
            {
                ((ICommand)commands[i]).UnExecute();
            }
        }
        
        public IRepeatableCommand GetRepeatCommand()
        {
            RepeatCommand rc = new RepeatCommand();
            rc.commands = new IRepeatableCommand[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                rc.commands[i] = commands[i].GetRepeatCommand();
            }
            return rc;
        }
    }
}
