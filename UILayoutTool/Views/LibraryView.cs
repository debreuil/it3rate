using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DDW.Display;
using DDW.Managers;
using DDW.Swf;
using Vex = DDW.Vex;
using System.Collections.Generic;
using DDW.Gdi;
using DDW.Interfaces;
using DDW.Commands;
using DDW.Utils;

namespace DDW.Views
{
    public partial class LibraryView : Form, IEditableView
    {
        private static LibraryView instance;
        public static LibraryView Instance { get { if (instance == null) instance = new LibraryView(); return instance; } }

        ILibraryTreeNode selectedItem;

        private static List<LibraryView> libraryViews = new List<LibraryView>();

        private LibraryView()
        {
            libraryViews.Add(this);

            InitializeComponent();

            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            bmp.SetResolution(96, 96);
            pictureBox.Image = bmp;
            
            symbolTree.MouseDown += new MouseEventHandler(symbolTree_MouseDown);

            symbolTree.DragDrop += new DragEventHandler(symbolTree_DragDrop);
            symbolTree.DragEnter += new DragEventHandler(symbolTree_DragEnter);
            symbolTree.DragOver += new DragEventHandler(symbolTree_DragOver);
            symbolTree.ItemDrag += new ItemDragEventHandler(symbolTree_ItemDrag);
            symbolTree.GiveFeedback += new GiveFeedbackEventHandler(symbolTree_GiveFeedback);
            symbolTree.AfterSelect += new TreeViewEventHandler(symbolTree_AfterSelect);
            
            pictureBox.MouseDown += new MouseEventHandler(pictureBox_MouseDown);
            pictureBox.MouseUp += new MouseEventHandler(pictureBox_MouseUp);

            symbolTree.AfterLabelEdit += new NodeLabelEditEventHandler(symbolTree_AfterLabelEdit);

            splitter1.SplitterMoved += new SplitterEventHandler(splitter1_SplitterMoved);
            this.Resize += new EventHandler(LibraryView_Resize);

            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LibraryView_KeyDown);
            this.PreviewKeyDown += new PreviewKeyDownEventHandler(LibraryView_PreviewKeyDown);

            symbolTree.ImageList = MainForm.Instance.ElementIcons;
            symbolTree.HideSelection = false;
        }
        
        public static LibraryView CurrentLibraryView
        {
            get
            {
                return libraryViews[0];
            }
        }

        public bool HasEditFocus()
        {
            //return this.Focused && symbolTree.LabelEdit;
            return symbolTree.Focused;
        }

        public void LibraryView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.Focused && ((ModifierKeys & Keys.Control) != Keys.Control))
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                    case Keys.Up:
                    case Keys.Left:
                    case Keys.Right:
                        e.IsInputKey = true;
                        break;
                }
            }
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (selectedItem != null && selectedItem is LibraryTreeNode)
                    {
                        LibraryTreeNode ltn = (LibraryTreeNode)selectedItem;
                        MainForm.CurrentStage.CommandStack.Do(
                            new DeleteItemsFromLibraryCommand(new uint[]{ltn.item.DefinitionId}));
                    }
                    break;

                case Keys.L:
                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        MainForm.Instance.ShowLibraryView(sender, e);
                    }
                    break;
            }
        }
        public void OnKeyPress(object sender, KeyPressEventArgs e) { }
        public void OnKeyUp(object sender, KeyEventArgs e) { }

        public void LoadCurrentLibrary()
        {
            selectedItem = null;
            symbolTree.Nodes.Clear();
            Library lib = MainForm.CurrentLibrary;
            uint[] keys = lib.Keys;
            for (uint i = 0; i < keys.Length; i++)
			{
                LibraryItem li = lib[keys[i]];
                if (!(li.Definition is Vex.Symbol) && li.Definition.Id != MainForm.CurrentStage.RootId)
                {
                    AddItemToLibrary(li);
                }
            }
            this.Invalidate();
        }
        public void Clear()
        {
            symbolTree.Nodes.Clear();
            if (pictureBox.Image != null)
            {
                Graphics g = Graphics.FromImage(pictureBox.Image);
                GraphicsUnit unit = GraphicsUnit.Pixel;
                g.FillRectangle(Brushes.White, pictureBox.Image.GetBounds(ref unit));
                g.Dispose();
                pictureBox.Invalidate();
            }
            selectedItem = null;
            this.Invalidate();
        }
        public void CheckForUpdates()
        {
            bool hasUpdates = false;
            CheckNodesForUpdates(symbolTree.Nodes, ref hasUpdates);
            if (hasUpdates)
            {
                symbolTree.Invalidate();
            }
        }
        private void CheckNodesForUpdates(TreeNodeCollection col, ref bool hasUpdates)
        {
            foreach (TreeNode tn in col)
            {
                if (tn is ILibraryTreeNode)
                {
                    ILibraryTreeNode ltn = (ILibraryTreeNode)tn;
                    if (ltn.CanUpdate)
                    {
                        string path = MainForm.CurrentStage.WorkingFolderFull + tn.FullPath;
                        if (File.Exists(path))
                        {
                            DateTime curTime = File.GetLastWriteTimeUtc(path);
                            if (ltn.Date.AddSeconds(5) < curTime)
                            {
                                ltn.NeedsUpdate = true;
                                hasUpdates = true;
                            }
                        }
                    }
                    else if (tn.Nodes.Count > 0)
                    {
                        CheckNodesForUpdates(tn.Nodes, ref hasUpdates);
                    }
                }
            }
        }

        Point mouseDownOrigin = Point.Empty;
        
        void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectedItem != null)
            {
                mouseDownOrigin = new Point(e.X, e.Y);
                pictureBox.MouseMove -= new MouseEventHandler(pictureBox_MouseMove);
                pictureBox.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
            }
        }
        void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (Math.Abs(e.X - mouseDownOrigin.X) > SystemInformation.DragSize.Width ||
                Math.Abs(e.Y - mouseDownOrigin.Y) > SystemInformation.DragSize.Height)
            {
                pictureBox.MouseMove -= new MouseEventHandler(pictureBox_MouseMove);
                startDragSelectedItem();
            }
        }
        void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox.MouseMove -= new MouseEventHandler(pictureBox_MouseMove);
 	        mouseDownOrigin = Point.Empty;
        }

        private void startDragSelectedItem()
        {
            if (selectedItem != null)
            {
				LibraryItemDragPacket packet = new LibraryItemDragPacket (selectedItem.Items);
				symbolTree.DoDragDrop(packet, DragDropEffects.Link);
            }
        }
        void symbolTree_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = e.Effect != DragDropEffects.Copy;
        }
        void symbolTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            startDragSelectedItem();
        }                
        void symbolTree_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode tn = symbolTree.GetNodeAt(e.Location);
            if (tn != null)
            {
                if (tn == selectedItem)
                {
                    symbolTree.LabelEdit = true;
                    //tn.BeginEdit();
                }               
                SelectNode(tn);
            }
        }
        void symbolTree_DragOver(object sender, DragEventArgs e)
        {
        }
        void symbolTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                bool hif = Library.HasImportableFile((string[])e.Data.GetData(DataFormats.FileDrop));
                if (hif)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        void symbolTree_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                this.Focus();

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Library.HasImportableFile(files))
                {
                    foreach (string file in files)
                    {
                        string ext = Path.GetExtension(file).ToUpperInvariant();
                        if (Library.ImportableFileExtensions.IndexOf(ext) > -1)
                        {
                            MainForm.CurrentStage.CommandStack.Do(new ImportFileCommand(file));
                        }
                    }
                }
            }
        }

        void LibraryView_Resize(object sender, EventArgs e)
        {
            splitter1_SplitterMoved(sender, e);
        }
        void splitter1_SplitterMoved(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
            }
            if (pictureBox.Width > 0 && pictureBox.Height > 0) // in case minimised
            {
                Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
                bmp.SetResolution(96, 96);
                pictureBox.Image = bmp;

                ResetImage();
            }
        }

        void symbolTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            symbolTree.LabelEdit = false;

            if (e.Node is LibraryTreeNode && e.Label != null && e.Label != "")
            {
                LibraryTreeNode liv = (LibraryTreeNode)e.Node;
                if (liv.item.Name != e.Label)
                {
                    liv.item.Name = e.Label;
                    MainForm.CurrentStage.HasSaveableChanges = true;
                }
            }
        }

        public void SelectFirstNode()
        {
            if (symbolTree != null && symbolTree.GetNodeCount(false) > 0)
            {
                SelectNode(symbolTree.Nodes[0]);
            }
        }
        public void SelectNode(uint libraryId)
        {
            if (symbolTree != null)
            {
                TreeNode node = null;
                FindNode(symbolTree.Nodes, libraryId, ref node);
                if (node != null)
                {
                    SelectNode(node);
                }
            }
        }
        public TreeNode GetSelectedNode()
        {
            TreeNode result = null;
            if (symbolTree != null)
            {
                result = symbolTree.SelectedNode;
            }
            return result;
        }
        public void RefreshCurrentNode()
        {
            SelectNode(GetSelectedNode());
        }
        public void SelectNode(TreeNode tn)
        {
            symbolTree.SelectedNode = tn;
        }
        void symbolTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedItem = (ILibraryTreeNode)symbolTree.SelectedNode;
            symbolTree.Update();
            ResetImage();
        }
        private void FindNode(TreeNodeCollection nodes, uint libraryId, ref TreeNode result)
        {
            foreach (TreeNode tn in nodes)
            {
                if (result != null)
                {
                    break;
                }

                if (tn is LibraryTreeNode && ((LibraryTreeNode)tn).item.DefinitionId == libraryId)
                {
                    result = tn;
                    break;
                }

                if (tn.Nodes != null && tn.Nodes.Count > 0)
                {
                    FindNode(tn.Nodes, libraryId, ref result);
                }
            }
        }
        public TreeNode FindNode(uint libraryId)
        {
            TreeNode result = null;
            FindNode(symbolTree.Nodes, libraryId, ref result);
            return result;
        }
        public bool InsertNode(LibraryTreeNode node, string path, int index)
        {
            bool result = false;
            TreeNodeCollection parentNodes = symbolTree.Nodes;
            int lastIndex = path.LastIndexOf(symbolTree.PathSeparator);
            if (lastIndex > -1)
            {
                string rootPath = path.Substring(0, lastIndex);
                TreeNode[] targ = symbolTree.Nodes.Find(rootPath, true);
                if(targ.Length > 0 && targ[0].Nodes.Count >= index)
                {
                    parentNodes = targ[0].Nodes;
                }
            }

            if(parentNodes != null)
            {
                parentNodes.Insert(index, node);
                result = true;
            }
            return result;
        }

        public void ResetImage()
        {
            if (selectedItem != null)
            {
                ILibraryTreeNode ltn = selectedItem;
                ltn.RenderThumbnail((Bitmap)pictureBox.Image);
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox.Invalidate();
            }
        }

        private TreeNodeCollection GetFolder(string fullPath)
        {
            TreeNodeCollection result = symbolTree.Nodes;

            if (fullPath != null && fullPath != "")
            {
                string[] folders = fullPath.Split(new string[] { symbolTree.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < folders.Length; i++)
                {
                    if (result.ContainsKey(folders[i]))
                    {
                        result = result[folders[i]].Nodes;
                    }
                    else
                    {
                        FolderTreeNode newFolder = new FolderTreeNode(folders[i], result.GetType(), 3, 4);
                        newFolder.Name = folders[i];
                        result.Add(newFolder);
                        newFolder.ResetDate();
                        result = newFolder.Nodes;
                        newFolder.Expand();
                    }
                }
            }
            return result;
        }

        public TreeNode AddItemToLibrary(LibraryItem li)
        {
            TreeNodeCollection nodes = GetFolder(li.LibraryPath);

            if (li.Name == null)
            {
                li.Name = MainForm.CurrentLibrary.GetNextDefaultName();
            }
            int imageIndex = GetImageIndex(li);
            LibraryTreeNode ltn = new LibraryTreeNode(li, imageIndex, imageIndex);
            ltn.Name = li.Name;
            nodes.Add(ltn);
            li.LibraryPath = (ltn.Parent != null) ? ltn.Parent.FullPath : "";
            return ltn;
        }
        public LibraryItem RemoveItemFromLibrary(uint id)
        {
            LibraryItem li = MainForm.CurrentLibrary[id];
            if (li != null)
            {
                TreeNode tn = null;
                FindNode(symbolTree.Nodes, id, ref tn);

                if (tn != null && tn is LibraryTreeNode)
                {
                    LibraryTreeNode liv = (LibraryTreeNode)tn;
                    MainForm.CurrentLibrary.RemoveLibraryItem(liv.item);
                    Console.WriteLine("remove " + liv.item.DefinitionId);
                    tn.Remove();
                }
                li.LibraryPath = "";
            }
            RefreshCurrentNode();

            uint[] uis = MainForm.CurrentLibrary.FindAllUsagesOfDefinition(id);
            UsageIdentifier[] removed = MainForm.CurrentStage.RemoveInstancesByIdGlobal(uis);
            MainForm.CurrentStage.InvalidateAll();

            return li;
        }

        public LibraryItem AddImage(string path)
        {
            LibraryItem result;
            string imgName = Path.GetFileName(path);

            string wf = MainForm.CurrentStage.WorkingFolderFull;
            bool inWorkingFolder = path.IndexOf(wf) == 0;
            string pathAndName = inWorkingFolder ? path.Substring(wf.Length) : imgName;
            string workingPath = pathAndName.Substring(0, pathAndName.Length - imgName.Length);

            //bool isUpdatingImage = MainForm.CurrentLibrary.HasPath(img.WorkingPath, img.Name);
            TreeNode existingNode = symbolTree.GetNode(workingPath, imgName);
            bool isUpdatingImage = (existingNode != null);

            Vex.Image img = new Vex.Image(path);
            img.Name = imgName;

            if (isUpdatingImage)
            {
                result = MainForm.CurrentLibrary.GetLibraryItem(workingPath, imgName);
                img.Id = result.DefinitionId;
                MainForm.CurrentStage.vexObject.Definitions[img.Id] = img;
                result.Definition = img;
                MainForm.CurrentStage.BitmapCache.RemoveBitmap(img);
            }
            else
            {
                result = MainForm.CurrentStage.CreateLibraryItem(img, false);
                result.LibraryPath = workingPath;
                AddItemToLibrary(result);
            }
             
            result.EnsureImageLoaded();
            DateTime time = File.GetLastAccessTimeUtc(path);
            result.Date = time;

            return result;
        }
        public LibraryItem[] AddSwf(string path)
        {
            List<LibraryItem> result = new List<LibraryItem>();

            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs);

                    string name = Path.GetFileNameWithoutExtension(path);
                    SwfReader r = new SwfReader(br.ReadBytes((int)fs.Length));

                    SwfCompilationUnit scu = new SwfCompilationUnit(r);

                    if (scu.IsValid)
                    {
                        SwfToVex s2v = new SwfToVex();
                        Vex.VexObject v = s2v.Convert(scu);
                        DateTime creationTime = File.GetLastWriteTimeUtc(path);

                        string swfName = Path.GetFileName(path);

                        string wf = MainForm.CurrentStage.WorkingFolderFull;
                        bool inWorkingFolder = path.IndexOf(wf) == 0;
                        string wp = inWorkingFolder ? path.Substring(wf.Length) : swfName;
                        string workingPath = wp.Substring(0, wp.Length - swfName.Length);

                        TreeNode existingNode = symbolTree.GetNode(workingPath, swfName);
                        bool isUpdatingSwf = (existingNode != null);

                        LibraryItem li;
                        Dictionary<uint, uint> translator = new Dictionary<uint, uint>();
                        List<uint> recycledInstIds = new List<uint>();
                        foreach (uint key in v.Definitions.Keys)
                        {
                            Vex.IDefinition loadedDef = v.Definitions[key];
                            if (scu.SymbolClasses.ContainsKey(key))
                            {
                                loadedDef.Name = scu.SymbolClasses[key];
                            }
                            uint loadedId = loadedDef.Id;
                            bool isTimeline = loadedDef is Vex.Timeline;

                            LibraryItem existingLi = null;
                            if (isUpdatingSwf)
                            {
                                loadedDef.HasSaveableChanges = true;
                                existingLi = isTimeline ? MainForm.CurrentLibrary.GetLibraryItem(workingPath + swfName, loadedDef.Name) : MainForm.CurrentLibrary.GetByOriginalSourceId(loadedDef.Id);
                            }

                            if (existingLi != null)
                            {                                
                                loadedDef.Id = existingLi.DefinitionId;
                                translator[loadedId] = loadedDef.Id;
                                existingLi.Date = creationTime;
                                if (isTimeline)
                                {
                                    Vex.Timeline orgTl = (Vex.Timeline)existingLi.Definition;
                                    Vex.IInstance[] instances = orgTl.Instances.ToArray();
                                    for (int i = 0; i < instances.Length; i++)
                                    {
                                        recycledInstIds.Add(instances[i].InstanceHash);
                                        orgTl.RemoveInstance(instances[i]);
                                        MainForm.CurrentStage.InstanceManager.RemoveInstance(instances[i].InstanceHash);
                                    }
                                }
                                MainForm.CurrentStage.vexObject.Definitions[loadedDef.Id] = loadedDef;
                                existingLi.Definition = loadedDef;
                            }
                            else
                            {
                                li = MainForm.CurrentStage.CreateLibraryItem(loadedDef, false);
                                li.Date = creationTime;
                                li.LibraryPath = workingPath + swfName;
                                li.OriginalSourceId = loadedId;
                                translator[loadedId] = li.Definition.Id;

                                if (isTimeline)
                                {
                                    result.Add(li);
                                    AddItemToLibrary(li);
                                }

                            }

                            if (isTimeline)
                            {
                                Vex.Timeline tl = (Vex.Timeline)loadedDef;
                                for (int i = 0; i < tl.InstanceCount; i++)
                                {
                                    Vex.IInstance inst = tl.InstanceAt(i);
                                    if (recycledInstIds.Count > 0)
                                    {
                                        inst.InstanceHash = recycledInstIds[0];
                                        recycledInstIds.RemoveAt(0);
                                    }

                                    inst.DefinitionId = translator[inst.DefinitionId];
                                    inst.ParentDefinitionId = translator[inst.ParentDefinitionId];
                                    MainForm.CurrentStage.CreateInstance((Vex.Instance)inst);

                                }
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }
        public int GetImageIndex(LibraryItem li)
        {
            int result = (int)LibraryIconIndex.Symbol;

            if(li.Definition is Vex.Image)
            {
                result = (int)LibraryIconIndex.Image;
            }
            return result;
        }
    }

    public enum LibraryIconIndex
    {
        Image = 0,
        Symbol = 5,
    }
}
