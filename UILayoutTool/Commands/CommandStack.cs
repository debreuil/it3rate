using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;

namespace DDW.Commands
{
    public class CommandStack
    {
        public event EventHandler OnUndoStackChanged;

        private List<ICommand> stack = new List<ICommand>();
        private int index = 0;

        public CommandStack()
        {
        }

        public int Count { get { return stack.Count; } }
        
        public bool CanUndo()
        {
            return index > 0;            
        }
        public bool CanRedo()
        {
            return (stack.Count > 0) && (index < stack.Count);
        }

        public void Do(ICommand item)
        {
            // delete any potential redo items
            if (stack.Count > index)
            {
                stack.RemoveRange(index, stack.Count - index);
            }

            stack.Add(item);
            item.Execute();

            index = stack.Count;
            OnUndoStackChanged(this, EventArgs.Empty);

            if (item is ISaveableCommand)
            {
                MainForm.CurrentStage.HasSaveableChanges = true;
            }
        }
        public bool Undo()
        {
            bool result = false;
            if (CanUndo())
            {
                index--;
                stack[index].UnExecute();
                result = true;
            }
            OnUndoStackChanged(this, EventArgs.Empty);
            return result;
        }
        public bool Redo()
        {
            bool result = false;
            if (CanRedo())
            {
                stack[index].Execute();
                index++;
                result = true;
            }
            OnUndoStackChanged(this, EventArgs.Empty);
            return result;
        }

        public ICommand Peek()
        {
            return (stack.Count > 0 && index > 0) ? stack[index - 1] : null;
        }

        public bool CanRepeat()
        {
            return stack.Count > 0 && stack[index] is IRepeatableCommand;
        }

        public IRepeatableCommand[] GetRepeatables()
        {
            List<IRepeatableCommand> commands = new List<IRepeatableCommand>();
            int i = index - 1;
            while (i >= 0 && stack[i] is IRepeatableCommand)
            {
                commands.Add(((IRepeatableCommand)stack[i]).GetRepeatCommand());
                if (stack[i] is RepeatCommand || stack[i] is DuplicateSelectedCommand)
                {
                    break;
                }
                i--;
            }
            commands.Reverse();
            return commands.ToArray();
        }
    }
}
