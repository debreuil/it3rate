using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Display;
using System.IO;
using System.Windows.Forms;
using DDW.Managers;
using DDW.Utils;

namespace DDW.Commands
{
    public class DeleteItemsFromLibraryCommand : ICommand, ISaveableCommand
    {
        uint[] libraryIds;
        UsageIdentifier[] removedInstances;
        LibraryTreeNode[] removedTreeNodes;
        string[] removedPaths;
        int[] removedIndexes;

        TreeNode selectedNode;

        public DeleteItemsFromLibraryCommand(uint[] libraryIds)
        {
            this.libraryIds = libraryIds;
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            Library lib = MainForm.CurrentLibrary;
            LibraryView libView = MainForm.CurrentLibraryView;

            List<LibraryTreeNode> remNodes = new List<LibraryTreeNode>();
            List<string> remPaths = new List<string>();
            List<int> remIndexes = new List<int>();

            selectedNode = libView.GetSelectedNode();

            for (int i = 0; i < libraryIds.Length; i++)
			{
                uint id = libraryIds[i];

                uint[] uis = lib.FindAllUsagesOfDefinition(id);
                removedInstances = stage.RemoveInstancesByIdGlobal(uis);

                LibraryItem li = lib[id];
                if (li != null)
                {
                    TreeNode tn = libView.FindNode(id);

                    if (tn != null && tn is LibraryTreeNode)
                    {
                        LibraryTreeNode liv = (LibraryTreeNode)tn;
                        remNodes.Add(liv);
                        remPaths.Add(liv.FullPath);
                        remIndexes.Add(liv.Index);

                        lib.RemoveLibraryItem(liv.item);
                        tn.Remove();
                    }
                }
            }

            removedTreeNodes = remNodes.ToArray();
            removedPaths = remPaths.ToArray();
            removedIndexes = remIndexes.ToArray();
            stage.InvalidateAll();
            libView.RefreshCurrentNode();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            Library lib = MainForm.CurrentLibrary;
            LibraryView libView = MainForm.CurrentLibraryView;

            for (int i = removedTreeNodes.Length - 1; i >= 0; i--)
            {   
                LibraryTreeNode liv = removedTreeNodes[i];
                lib.AddLibraryItem(liv.item);
                libView.InsertNode(liv, removedPaths[i], removedIndexes[i]);
            }

            // todo: need to readd bonds and snaps.
            for (int i = removedInstances.Length - 1; i >= 0; i--)
            {
                stage.AddInstance(removedInstances[i]);
            }

            libView.SelectNode(selectedNode);


            removedInstances = null;
            removedTreeNodes = null;
            removedPaths = null;
            removedIndexes = null;
            selectedNode = null;

            stage.InvalidateAll();
            libView.RefreshCurrentNode();
        }
    }
}
