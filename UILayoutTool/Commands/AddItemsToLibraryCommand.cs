using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Display;

namespace DDW.Commands
{
    public class AddItemsToLibraryCommand : ICommand, ISaveableCommand
    {
        Vex.IDefinition[] defintions;
        uint newLibraryItemId;

        public AddItemsToLibraryCommand(Vex.IDefinition[] defintion)
        {
            this.defintions = defintion;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;

            for (int i = 0; i < defintions.Length; i++)
            {
                LibraryItem li = stage.CreateLibraryItem(defintions[i], true);
                newLibraryItemId = li.DefinitionId;
                stage.Library.AddLibraryItem(li);
                LibraryView.CurrentLibraryView.AddItemToLibrary(li);
            }
        }

        public void UnExecute()
        {
            LibraryView.CurrentLibraryView.RemoveItemFromLibrary(newLibraryItemId);
        }
    }
}
