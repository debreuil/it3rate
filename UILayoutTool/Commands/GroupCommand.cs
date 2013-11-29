using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;

namespace DDW.Commands
{
    class GroupCommand : ICommand, IRepeatableCommand, ISaveableCommand
    {
        IRepeatableCommand[] commands;

        public GroupCommand(params IRepeatableCommand[] commands)
        {
            this.commands = commands;
        }

        public void Execute()
        {
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
            return new GroupCommand(commands);
        }
    }
}
