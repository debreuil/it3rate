using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DDW.Views;
using DDW.Commands;
using DDW.Display;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using DDW.Controls;
using DDW.Managers;
using DDW.VectorML;
using DDW.VexDraw;
using System.Diagnostics;

namespace DDW
{
    public partial class MainForm : Form
    {
        private string testFile = @"examples\test\test.uil";
        private static MainForm inst;
        public static MainForm Instance { get { return inst; } }

        private List<StageView> stages = new List<StageView>();
        private static StageView currentStage;

        public static StageView CurrentStage { get { return currentStage; } }
        public static Library CurrentLibrary { get { return (currentStage == null) ? null : currentStage.Library; } }
        public static InstanceManager CurrentInstanceManager { get { return (currentStage == null) ? null :  currentStage.InstanceManager; } }

        private static LibraryView currentLibraryView;
        public static LibraryView CurrentLibraryView { get { return currentLibraryView; } }

        private static PropertyBar propertyBar;
        public static PropertyBar PropertyBar { get { return propertyBar; } }

        public ImageList ElementIcons { get { return elementIcons; } }

        public MainForm()
        {
            inst = this;
            InitializeComponent();

            propertyBar = new PropertyBar();
            propertyBar.Dock = DockStyle.Bottom;
            propertyBar.TopLevel = false;
            inst.Controls.Add(propertyBar);
            propertyBar.Show();

            //propertyBar.Show(dockPanel, propertyBarDocState);
            //propertyBar.DockHandler.DockAreas = DockAreas.DockBottom;
            //propertyBar.DockHandler.AutoHideButtonVisible = false;
            //propertyBar.DockHandler.HideOnClose = true;

            currentLibraryView = LibraryView.Instance;
            currentLibraryView.Dock = DockStyle.Right;
            currentLibraryView.TopLevel = false;
            inst.Controls.Add(currentLibraryView);
            currentLibraryView.Show();

            //currentLibraryView.Show(dockPanel, libraryDocState);
            //currentLibraryView.DockHandler.HideOnClose = true;

            //dockPanel.UpdateDockWindowZOrder(DockStyle.Right, true);
            //dockPanel.UpdateDockWindowZOrder(DockStyle.Bottom, false);

            this.DoubleBuffered = true;
            this.AddEvents();
#if DEBUG
            if (File.Exists(testFile))
            {
                string fullPath = Path.GetFullPath(testFile);
                CreateNewStage(Path.GetFileNameWithoutExtension(fullPath));
                currentStage.LoadUIL(fullPath);
                currentLibraryView.SelectFirstNode();
            }
            else
            {
                NewDocument(this, EventArgs.Empty);
            }
#else
            NewDocument(this, EventArgs.Empty);
#endif
        }

        void MainForm_Activated(object sender, EventArgs e)
        {
            if (CurrentLibraryView != null)
            {
                CurrentLibraryView.CheckForUpdates();
            }
        }

        private void AddEvents()
        {
            levelBar.ItemClicked += new ToolStripItemClickedEventHandler(levelBar_ItemClicked);
            this.Activated += new EventHandler(MainForm_Activated);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
        }
        private void RemoveEvents()
        {
            if (levelBar != null)
            {
                levelBar.ItemClicked -= new ToolStripItemClickedEventHandler(levelBar_ItemClicked);
            }
            this.Activated -= new EventHandler(MainForm_Activated);
            this.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
        }
        private void CreateNewStage(string name)
        {
            Library lb = new Library();
            InstanceManager im = new InstanceManager();
            StageView sv = new StageView(lb, im, name);
            sv.GotFocus += new EventHandler(stageView_GotFocus);
            sv.LostFocus += new EventHandler(stageView_LostFocus);
            stages.Add(sv);
            SetStage(sv);
        }

        private void SetStage(StageView sv)
        {
            if (currentStage != null)
            {
                currentLibraryView.Clear();

                currentStage.OnSelectionChanged -= OnSelectionChanged;
                currentStage.OnUndoStackChanged -= OnUndoStackChanged;
                currentStage.OnNestedEditChanged -= OnNestedEditChanged;
                currentStage.FormClosing -= new FormClosingEventHandler(currentStage_FormClosing);
                currentStage.OnDepthChanged -= OnDepthChanged;
                currentStage = null;
            }

            if (sv != null)
            {
                if (sv.rootFolder != null)
                {
                    Environment.CurrentDirectory = sv.rootFolder;
                }

                currentStage = sv;
                currentLibraryView.LoadCurrentLibrary();

                currentStage.TopLevel = false;
                currentStage.Dock = DockStyle.Fill;
                inst.Controls.Add(currentStage);
                currentStage.Show();

                //sv.Show(dockPanel);

                currentStage.OnSelectionChanged += OnSelectionChanged;
                currentStage.OnUndoStackChanged += OnUndoStackChanged;
                currentStage.OnNestedEditChanged += OnNestedEditChanged;
                currentStage.OnDepthChanged += OnDepthChanged;
                currentStage.FormClosing += new FormClosingEventHandler(currentStage_FormClosing);
                currentStage.Invalidate();

                saveToolStripMenuItem.Enabled = currentStage.HasSaveableChanges;
                importToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                closeToolStripMenuItem.Enabled = true;
                exportSVGToolStripMenuItem.Enabled = true;
                exportXAMLToolStripMenuItem.Enabled = true;
                exportCanvasToolStripMenuItem.Enabled = true;
            }
            else
            {
                saveToolStripMenuItem.Enabled = false;
                importToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                closeToolStripMenuItem.Enabled = false;
                exportSVGToolStripMenuItem.Enabled = false;
                exportXAMLToolStripMenuItem.Enabled = false;
                exportCanvasToolStripMenuItem.Enabled = false;
            }
        }

        private IEditableView GetFocusedView()
        {
            IEditableView result;
            if (propertyBar.HasEditFocus())
            {
                result = propertyBar;
            }
            else if (currentLibraryView.HasEditFocus())
            {
                result = currentLibraryView;
            }
            else
            {
                result = currentStage;
            }
            return result;
        }
        public void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (((ModifierKeys & Keys.Control) != Keys.Control))
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
            GetFocusedView().OnKeyDown(sender, e);
        }
        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            GetFocusedView().OnKeyPress(sender, e);
        }
        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            GetFocusedView().OnKeyUp(sender, e);
        }

        void stageView_GotFocus(object sender, EventArgs e)
        {
            if (sender is StageView)
            {
                StageView sv = (StageView)sender;
                if (sender != currentStage)
                {
                    SetStage((StageView)sender);
                }
                else
                {
                    if (sv.Selection != null)
                    {
                        OnSelectionChanged(sv, sv.Selection.Count);
                    }
                }
            }
        }
        void stageView_LostFocus(object sender, EventArgs e)
        {
            if (sender is StageView)
            {
                StageView sv = (StageView)sender;
                OnSelectionChanged(sv, 0);
            }
        }
        private int untitledDocumentIndex = 1;
        public void NewDocument(object sender, EventArgs e)
        {
            CreateNewStage("Untitled" + untitledDocumentIndex++);
            currentStage.CreateRoot();
            //currentLibraryView.OpenAssets();
        }
        private void OpenDocument(object sender, EventArgs e)
        {
            LoadUIL();
        }
        private void ImportToStage(object sender, EventArgs e)
        {
            Import();
        }
        public void CloseDocument(object sender, EventArgs e)
        {
            if (currentStage != null)
            {
                currentStage.Close();
            }
        }
        private void OpenInExplorer(object sender, EventArgs e)
        {
            if (CurrentStage != null && Directory.Exists(CurrentStage.rootFolder))
            {
                Process.Start(CurrentStage.rootFolder);
            }
            else
            {
                Process.Start(Environment.CurrentDirectory);
            }
        }
        private void OpenWorkingFolder(object sender, EventArgs e)
        {
            if (CurrentStage != null && Directory.Exists(CurrentStage.WorkingFolderFull))
            {
                Process.Start(CurrentStage.WorkingFolderFull);
            }
        }
        private void ExportSVG(object sender, EventArgs e)
        {
            string path = CurrentStage.ExportFolderFull;
            string svgFileName;
            MLRenderer xr = new SVGRenderer();
            xr.GenerateML(currentStage.vexObject, path, out svgFileName);
        }
        private void ExportXAML(object sender, EventArgs e)
        {
            string path = CurrentStage.ExportFolderFull;
            string xamlFileName;
            MLRenderer xr = new WPFRenderer();
            xr.GenerateML(currentStage.vexObject, path, out xamlFileName);
        }
        private void ExportDirectX(object sender, EventArgs e)
        {
            Vex.LoadFromUIL.Load(testFile);
        }
        private void ExportCanvasData(object sender, EventArgs e)
        {
            VexDrawDataGenerator vddg = new VexDrawDataGenerator();

            string path = CurrentStage.ExportFolderFull;
            string fileName;
            vddg.GenerateBinaryData(currentStage.vexObject, path, out fileName);
            vddg.GenerateJsonData(currentStage.vexObject, path, out fileName);
        }

        private void SaveDocument(object sender, EventArgs e)
        {
            Save();
        }
        private void SaveDocumentAs(object sender, EventArgs e)
        {
            SaveAs();
        }
        private void ExitApplication(object sender, EventArgs e)
        {
            this.Close();
        }

        public void DocumentUndo(object sender, EventArgs e)
        {
            currentStage.Undo();
        }
        public void DocumentRedo(object sender, EventArgs e)
        {
            currentStage.Redo();
        }

        private void SelectionCut(object sender, EventArgs e)
        {

        }
        private void SelectionCopy(object sender, EventArgs e)
        {

        }
        private void SelectionPaste(object sender, EventArgs e)
        {
            //IDataObject ido = Clipboard.GetDataObject();
            //string[] formats = ido.GetFormats();
            //byte[][] data = new byte[formats.Length][];
            //for (int i = 0; i < formats.Length; i++)
            //{
            //    object o = ido.GetData(formats[i]);
            //    if (o != null && o is MemoryStream)
            //    {
            //        MemoryStream ms = (MemoryStream)o;
            //        byte[] d = new byte[ms.Length];
            //        ms.Read(d, 0, (int)ms.Length);
            //        data[i] = d;
            //    }
            //}
        }
        public void SelectionDelete(object sender, EventArgs e)
        {
            currentStage.Delete();
        }
        private void SelectionDuplicate(object sender, EventArgs e)
        {
            currentStage.Duplicate();
        }

        private void DocumentSelectAll(object sender, EventArgs e)
        {
            currentStage.SelectAll();
        }
        public void DocumentSelectNone(object sender, EventArgs e)
        {
            currentStage.SelectNone();
        }

        private void DocumentEditInPlace(object sender, EventArgs e)
        {
            currentStage.EditInPlace();
        }
        private void DocumentFinishEditInPlace(object sender, EventArgs e)
        {
            currentStage.FinishEditInPlace(1);
        }
        void levelBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int index = levelBar.Items.IndexOf(e.ClickedItem);
            currentStage.FinishEditInPlace(levelBar.Items.Count - 1 - index);
        }

        private void DocumentZoomIn(object sender, EventArgs e)
        {
            currentStage.ZoomSelection(4);
        }
        private void DocumentZoomOut(object sender, EventArgs e)
        {
            currentStage.ZoomSelection(-4);
        }
        private void DocumentZoom100Percent(object sender, EventArgs e)
        {
            currentStage.ResetZoom();
        }
        private void DocumentZoomToFit(object sender, EventArgs e)
        {
            currentStage.ZoomToFit();
        }
        private void DocumentZoomToSelection(object sender, EventArgs e)
        {
            if (currentStage.Selection.Count > 0)
            {
                currentStage.ZoomToSelection();
            }
            else
            {
                currentStage.ZoomToFit();
            }
        }

        private void ShowRulers(object sender, EventArgs e)
        {
            currentStage.ShowRulers = !currentStage.ShowRulers;
        }
        private void ShowGrid(object sender, EventArgs e)
        {
            currentStage.ShowGrid = !currentStage.ShowGrid;
        }
        private void ShowGuides(object sender, EventArgs e)
        {
            currentStage.ShowGuides = !currentStage.ShowGuides;
            currentStage.InvalidateAll();
        }
        private void SnapToObjects(object sender, EventArgs e)
        {
            currentStage.SnapToObjects = !currentStage.SnapToObjects;
        }
        private void UseSmartBonds(object sender, EventArgs e)
        {
            currentStage.UseSmartBonds = !currentStage.UseSmartBonds;
        }

        //private DockState libraryDocState = DockState.DockRight;
        public void ShowLibraryView(object sender, EventArgs e)
        {
            if (currentLibraryView.Visible)
            {
                //libraryDocState = currentLibraryView.DockState;
                currentLibraryView.Hide();
            }
            else
            {
                //currentLibraryView.Show(dockPanel, libraryDocState);
                currentLibraryView.Show();
            }
            //currentLibraryView.Visible = !currentLibraryView.Visible;
        }

        //private DockState propertyBarDocState = DockState.DockBottom;
        public void ShowPropertyBar(object sender, EventArgs e)
        {
            if (propertyBar.Visible)
            {
                //propertyBarDocState = propertyBar.DockState;
                propertyBar.Hide();
            }
            else
            {
                //propertyBar.Show(dockPanel, propertyBarDocState);
                propertyBar.Show();
            }
        }

        private void SelectionNumericTransform(object sender, EventArgs e)
        {
        }
        private void SelectionFlipHorizontal(object sender, EventArgs e)
        {
            currentStage.FlipHorizontal();
        }
        private void SelectionFlipVertical(object sender, EventArgs e)
        {
            currentStage.FlipVertical();
        }
        private void SelectionRotateRight15Degrees(object sender, EventArgs e)
        {
            currentStage.Rotate(15);
        }
        private void SelectionRotateLeft15Degrees(object sender, EventArgs e)
        {
            currentStage.Rotate(-15);
        }
        private void SelectionRemoveTransform(object sender, EventArgs e)
        {
            currentStage.RemoveTransforms();
        }

        private void SelectionAlignToLeft(object sender, EventArgs e)
        {
            currentStage.AlignToLeft();
        }
        private void SelectionAlignToRight(object sender, EventArgs e)
        {
            currentStage.AlignToRight();
        }
        private void SelectionAlignToTop(object sender, EventArgs e)
        {
            currentStage.AlignToTop();
        }
        private void SelectionAlignToBottom(object sender, EventArgs e)
        {
            currentStage.AlignToBottom();
        }
        private void SelectionAlignHorizontalCenters(object sender, EventArgs e)
        {
            currentStage.AlignHorizontalCenter();
        }
        private void SelectionAlignVerticalCenters(object sender, EventArgs e)
        {
            currentStage.AlignVerticalCenter();
        }
        private void SelectionDistributeToLastAlign(object sender, EventArgs e)
        {
            currentStage.DistributeToLastAlign();
        }
        private void SelectionRemoveAllBonds(object sender, EventArgs e)
        {
            currentStage.RemoveBondsFromSelection();
        }

        private void SelectionToTop(object sender, EventArgs e)
        {
            currentStage.AdjustSelectionDepth(1, true, true);
        }
        private void SelectionToBottom(object sender, EventArgs e)
        {
            currentStage.AdjustSelectionDepth(-1, true, true);
        }
        private void SelectionUpOne(object sender, EventArgs e)
        {
            currentStage.AdjustSelectionDepth(1, false, true);
        }
        private void SelectionBackOne(object sender, EventArgs e)
        {
            currentStage.AdjustSelectionDepth(-1, false, true);
        }
        private void SelectionInfrontOf(object sender, EventArgs e)
        {

        }
        private void SelectionBehind(object sender, EventArgs e)
        {

        }

        private void SelectionGroup(object sender, EventArgs e)
        {

        }
        private void SelectionUngroup(object sender, EventArgs e)
        {

        }
        private void SelectionMakeContainer(object sender, EventArgs e)
        {
            currentStage.MakeContainer();
        }
        private void SelectionBreakApart(object sender, EventArgs e)
        {
            currentStage.BreakApart();
        }

        private void SelectionSwapObject(object sender, EventArgs e)
        {

        }
        private void SelectionDuplicateObject(object sender, EventArgs e)
        {
            currentStage.Duplicate();
        }

        private void ShowDocumentProperties(object sender, EventArgs e)
        {

        }

        private void OnlineHelp(object sender, EventArgs e)
        {

        }
        private void ShowShortcutKeysHelp(object sender, EventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var location = System.IO.Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar;
            location += @"documentation" + Path.DirectorySeparatorChar + "shortcutKeys.html";
            if (File.Exists(location))
            {
                System.Diagnostics.Process.Start(location);
            }
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            It3rateAbout about = new It3rateAbout();
            about.ShowDialog();
        }

        private int selectionCount;
        // has selection
        // one selected
        // all selected
        private void OnSelectionChanged(StageView sender, int count)
        {
            bool hasFocus = currentStage.HasEditFocus();
            int instCount = hasFocus ? currentStage.InstanceCount : 0;
            bool hasSelection = hasFocus ? count > 0 : false;

            if ( (selectionCount > 0 && count == 0) ||
                 (selectionCount == 0 && count > 0) ||
                  !hasFocus)
            {
                cutToolStripMenuItem.Enabled = hasSelection;
                copyToolStripMenuItem.Enabled = hasSelection;
                deleteMenuItem.Enabled = hasSelection;
                duplicateMenuItem.Enabled = hasSelection;
                selectNoneToolStripMenuItem.Enabled = hasSelection;

                flipHorizontalToolStripMenuItem.Enabled = hasSelection;
                flipVerticalToolStripMenuItem.Enabled = hasSelection;
                rotateLeft15ToolStripMenuItem.Enabled = hasSelection;
                rotateRight15ToolStripMenuItem.Enabled = hasSelection;
                removeTransformToolStripMenuItem.Enabled = hasSelection;
            }

            if (hasSelection)
            {
                breakApartToolStripMenuItem.Enabled = currentStage.CanBreakApart();
            }

            // isTop/isBottom is a bit expensive, so sanity test first
            if (hasSelection && instCount > count)
            {
                DepthChanged();
            }
            else
            {
                toTopToolStripMenuItem.Enabled = false;
                toBottomToolStripMenuItem.Enabled = false;
                upOneToolStripMenuItem.Enabled = false;
                backOneToolStripMenuItem.Enabled = false;
                infrontOfToolStripMenuItem.Enabled = false;
                behindToolStripMenuItem.Enabled = false;
            }

            editInPlaceToolStripMenuItem.Enabled = hasFocus && currentStage.CanEditInPlace();

            if(hasFocus && selectionCount == instCount)
            {
                selectAllToolStripMenuItem.Enabled = false;
            }
            else
            {
                selectAllToolStripMenuItem.Enabled = true;
            }

            selectionCount = count;
        }
        // undo stack change
        private void OnUndoStackChanged(Object sender, EventArgs e)
        {
            undoToolStripMenuItem.Enabled = currentStage.CanUndo();
            redoToolStripMenuItem.Enabled = currentStage.CanRedo();
            saveToolStripMenuItem.Enabled = currentStage.HasSaveableChanges;
        }
        // edit stack
        private void OnNestedEditChanged(Object sender, EventArgs e)
        {
            finishEditInPlaceToolStripMenuItem.Enabled = currentStage.CanFinishEditInPlace(1);
            levelBar.Items.Clear();

            int editLayers = currentStage.EditStack.Count;
            for (int i = 0; i < editLayers; i++)
            {
                DesignTimeline dt = currentStage.EditStack.ElementAt(editLayers - 1 - i);
                string name = (i == 0) ? currentStage.Name : dt.LibraryItem.Name;
                int imageIndex = LibraryView.Instance.GetImageIndex(dt.LibraryItem);
                
                ToolStripButton bt = new ToolStripButton(name, elementIcons.Images[imageIndex]);
                bt.Enabled = (i < editLayers - 1);
                levelBar.Items.Insert(i, bt);
            }

        }
        // depth change
        private void OnDepthChanged(Object sender, EventArgs e)
        {
            DepthChanged();
        }

        private void DepthChanged()
        {
            bool isTop = currentStage.IsTop();
            bool isBottom = currentStage.IsBottom();
            toTopToolStripMenuItem.Enabled = !isTop;
            toBottomToolStripMenuItem.Enabled = !isBottom;
            upOneToolStripMenuItem.Enabled = !isTop;
            backOneToolStripMenuItem.Enabled = !isBottom;
        }

        void currentStage_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = StartClosingDocument();
        }

        #region Load Save
        public bool Save()
        {
            bool result = false;
            if (currentStage != null)
            {
                if (currentStage.FileFullPathAndName != null && !currentStage.isNewFile)
                {
                    currentStage.Save(currentStage.FileFullPathAndName);
                }
                else
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "UIL Format|*.uil";
                    saveFileDialog1.Title = "Save UIL File";
                    saveFileDialog1.ShowDialog();

                    if (saveFileDialog1.FileName != "")
                    {
                        currentStage.Save(saveFileDialog1.FileName);
                    }
                }
                saveToolStripMenuItem.Enabled = currentStage.HasSaveableChanges;
            }
            return result;
        }
        public bool SaveAs()
        {
            bool result = false;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "UIL Format|*.uil";
            saveFileDialog1.Title = "Save UIL File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                currentStage.MarkAllHasSaveableChanges(true);
                currentStage.Save(saveFileDialog1.FileName);
            }
            saveToolStripMenuItem.Enabled = currentStage.HasSaveableChanges;

            return result;
        }
        public bool StartClosingDocument()
        {
            bool cancel = false;
            if (currentStage != null)
            {
                if (currentStage.HasSaveableChanges)
                {
                    DialogResult dr = MessageBox.Show("    Save changes to " + currentStage.Text + "?", "Unsaved Changes", MessageBoxButtons.YesNoCancel);
                    if (dr == DialogResult.Yes)
                    {
                        Save();
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        cancel = true;
                    }
                }

                if (!cancel)
                {
                    stages.Remove(currentStage);
                    currentStage.GotFocus -= new EventHandler(stageView_GotFocus);
                    currentStage.LostFocus -= new EventHandler(stageView_LostFocus);
                    currentStage.Hide();

                    if (stages.Count > 0)
                    {
                        SetStage(stages[0]);
                    }
                    else
                    {
                        SetStage(null);
                    }
                }
            }
            return cancel;
        }
        public bool LoadUIL()
        {
            bool result = false;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "UIL Format|*.uil";
            openFileDialog1.Title = "Open UIL File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                string name = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                CreateNewStage(name);
                currentStage.LoadUIL(openFileDialog1.FileName);
                //currentLibraryView.OpenAssets();

                currentLibraryView.SelectFirstNode();
            }

            return result;
        }
        public bool Import()
        {
            bool result = false;
            string curDir = Directory.GetCurrentDirectory();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string swf = "SWF Files(*.SWF)|*.SWF";
            string img = "Images(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            string uil = "UIL Files|*.UIL";
            string allFiles = "All Files|" + Library.ImportableFileExtensions;
            openFileDialog1.Filter = swf + "|" + img + "|" + uil + "|" + allFiles;
            openFileDialog1.FilterIndex = 4;
            openFileDialog1.Title = "Import File";
            openFileDialog1.InitialDirectory = currentStage.WorkingFolderFull;
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                currentStage.CommandStack.Do(new ImportFileCommand(openFileDialog1.FileName));
            }

            Directory.SetCurrentDirectory(curDir);
            
            return result;
        }

        public static List<LibraryItem> ImportFile(string filename)
        {
            List<LibraryItem> result = new List<LibraryItem>();
            string ext = Path.GetExtension(filename).ToUpperInvariant();
            if (ext == ".SWF")
            {
                LibraryItem[] items = currentLibraryView.AddSwf(filename);
                result.AddRange(items);
            }
            else if (ext == ".BMP" || ext == ".JPG" || ext == ".GIF" || ext == ".PNG")
            {
                LibraryItem item = currentLibraryView.AddImage(filename);
                result.Add(item);
            }

            if (currentLibraryView.GetSelectedNode() == null && result.Count > 0)
            {
                currentLibraryView.SelectNode(result[0].DefinitionId);
            }
            else
            {
                currentLibraryView.RefreshCurrentNode();
            }

            currentStage.HasSaveableChanges = true;

            currentStage.InvalidateAll();
            currentLibraryView.Invalidate();
            return result;
        }

        #endregion
    }

    [Flags]
    enum MenuCategory
    {
        CommandStack,
        Clipboard,
        Selection,
        EditLevel,

        Transform,

        Edit = CommandStack | Clipboard | Selection | EditLevel,
        Modify = Transform,

        All = 0xFFFF,
    }


}
