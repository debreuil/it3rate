using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Display;
using System.IO;

namespace DDW.Commands
{
    public class ImportFileCommand : ICommand, ISaveableCommand
    {
        string filename;
        LibraryItem[] libraryItems;

        Vex.Point location;
        bool addToStage = false;
        List<AddInstancesCommand> addInstanceCommands;

        public ImportFileCommand(string filename)
        {
            this.filename = filename;
        }
        public ImportFileCommand(string filename, Vex.Point location)
        {
            this.filename = filename;
            this.location = location;
            addToStage = true;
            addInstanceCommands = new List<AddInstancesCommand>();
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            LibraryView currentLibraryView = MainForm.CurrentLibraryView;

            string ext = Path.GetExtension(filename).ToUpperInvariant();
            if (ext == ".SWF")
            {
                libraryItems = currentLibraryView.AddSwf(filename);
            }
            else if (ext == ".BMP" || ext == ".JPG" || ext == ".GIF" || ext == ".PNG")
            {
                libraryItems = new LibraryItem[]{currentLibraryView.AddImage(filename)};
            }

            if (currentLibraryView.GetSelectedNode() == null && libraryItems.Length > 0)
            {
                currentLibraryView.SelectNode(libraryItems[0].DefinitionId);
            }
            else
            {
                currentLibraryView.RefreshCurrentNode();
            }

            if (addToStage)
            {
                uint[] itemIds = new uint[libraryItems.Length];
                Vex.Point[] locs = new Vex.Point[libraryItems.Length];

                for (int i = 0; i < libraryItems.Length; i++)
                {
                    itemIds[i] = libraryItems[i].Definition.Id;
                    Vex.Point centerOffset = libraryItems[i].Definition.StrokeBounds.Center.Negate();
                    locs[i] = location.Translate(centerOffset);
                }
                AddInstancesCommand aic = new AddInstancesCommand(itemIds, locs);
                aic.Execute();
                addInstanceCommands.Add(aic);
                stage.InvalidateAll();
            }

            stage.HasSaveableChanges = true;
            currentLibraryView.Invalidate();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            LibraryView currentLibraryView = MainForm.CurrentLibraryView;

            if (addToStage)
            {
                for (int i = 0; i < addInstanceCommands.Count; i++)
                {
                    addInstanceCommands[i].UnExecute();
                }
                addInstanceCommands.Clear();
                stage.InvalidateAll();
            }

            for (int i = 0; i < libraryItems.Length; i++)
            {
                currentLibraryView.RemoveItemFromLibrary(libraryItems[i].DefinitionId);
            }
            libraryItems = null;

            if (currentLibraryView.GetSelectedNode() == null && libraryItems.Length > 0)
            {
                currentLibraryView.SelectNode(libraryItems[0].DefinitionId);
            }
            else
            {
                currentLibraryView.RefreshCurrentNode();
            }


            stage.HasSaveableChanges = true;
            currentLibraryView.Invalidate();
        }
    }
}
