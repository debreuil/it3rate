namespace DDW
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                RemoveEvents();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
            WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.openInExplorerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWorkingFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportCanvasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSVGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportXAMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDirectXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAndroidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportIOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.editInPlaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.finishEditInPlaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Zoom100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToFitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.rulersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smartBondsStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.libraryViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numericTransformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateRight15ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateLeft15ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripSeparator();
            this.removeTransformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.alignDistributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignHorizontalCentersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignVerticalCentersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.distributeToLastAlignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripSeparator();
            this.removeAllBondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infrontOfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.behindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripSeparator();
            this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ungroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeContainerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakApartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripSeparator();
            this.swapContainerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateContainerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripSeparator();
            this.appPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shortcutKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem18 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelBar = new System.Windows.Forms.ToolStrip();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.elementIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolStripMenuItem10,
            this.commandsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1234, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.importToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openInExplorerMenuItem,
            this.openWorkingFolderMenuItem,
            this.toolStripSeparator2,
            this.exportCanvasToolStripMenuItem,
            this.exportSVGToolStripMenuItem,
            this.exportXAMLToolStripMenuItem,
            this.exportDirectXToolStripMenuItem,
            this.exportAndroidToolStripMenuItem,
            this.exportIOSToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewDocument);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenDocument);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.importToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToStage);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(261, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveDocument);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveDocumentAs);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(261, 6);
            // 
            // openInExplorerMenuItem
            // 
            this.openInExplorerMenuItem.Name = "openInExplorerMenuItem";
            this.openInExplorerMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.openInExplorerMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openInExplorerMenuItem.Text = "Open in Explorer";
            this.openInExplorerMenuItem.Click += new System.EventHandler(this.OpenInExplorer);
            // 
            // openWorkingFolderMenuItem
            // 
            this.openWorkingFolderMenuItem.Name = "openWorkingFolderMenuItem";
            this.openWorkingFolderMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.openWorkingFolderMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openWorkingFolderMenuItem.Text = "Open Working Folder";
            this.openWorkingFolderMenuItem.Click += new System.EventHandler(this.OpenWorkingFolder);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(261, 6);
            // 
            // exportCanvasToolStripMenuItem
            // 
            this.exportCanvasToolStripMenuItem.Name = "exportCanvasToolStripMenuItem";
            this.exportCanvasToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exportCanvasToolStripMenuItem.Text = "Export Canvas Data";
            this.exportCanvasToolStripMenuItem.Click += new System.EventHandler(this.ExportCanvasData);
            // 
            // exportSVGToolStripMenuItem
            // 
            this.exportSVGToolStripMenuItem.Name = "exportSVGToolStripMenuItem";
            this.exportSVGToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exportSVGToolStripMenuItem.Text = "Export to SVG";
            this.exportSVGToolStripMenuItem.Click += new System.EventHandler(this.ExportSVG);
            // 
            // exportXAMLToolStripMenuItem
            // 
            this.exportXAMLToolStripMenuItem.Name = "exportXAMLToolStripMenuItem";
            this.exportXAMLToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exportXAMLToolStripMenuItem.Text = "Export to XAML";
            this.exportXAMLToolStripMenuItem.Click += new System.EventHandler(this.ExportXAML);
            // 
            // exportDirectXToolStripMenuItem
            // 
            this.exportDirectXToolStripMenuItem.Enabled = false;
            this.exportDirectXToolStripMenuItem.Name = "exportDirectXToolStripMenuItem";
            this.exportDirectXToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exportDirectXToolStripMenuItem.Text = "Export to DirectX";
            this.exportDirectXToolStripMenuItem.Click += new System.EventHandler(this.ExportDirectX);
            // 
            // exportAndroidToolStripMenuItem
            // 
            this.exportAndroidToolStripMenuItem.Enabled = false;
            this.exportAndroidToolStripMenuItem.Name = "exportAndroidToolStripMenuItem";
            this.exportAndroidToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exportAndroidToolStripMenuItem.Text = "Export to Android";
            // 
            // exportIOSToolStripMenuItem
            // 
            this.exportIOSToolStripMenuItem.Enabled = false;
            this.exportIOSToolStripMenuItem.Name = "exportIOSToolStripMenuItem";
            this.exportIOSToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exportIOSToolStripMenuItem.Text = "Export to iOS";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(261, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseDocument);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(261, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitApplication);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripMenuItem4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.duplicateMenuItem,
            this.deleteMenuItem,
            this.toolStripMenuItem5,
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem,
            this.toolStripMenuItem8,
            this.editInPlaceToolStripMenuItem,
            this.finishEditInPlaceToolStripMenuItem,
            this.toolStripMenuItem9});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.DocumentUndo);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.DocumentRedo);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(241, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Enabled = false;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.SelectionCut);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.SelectionCopy);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.SelectionPaste);
            // 
            // duplicateMenuItem
            // 
            this.duplicateMenuItem.Name = "duplicateMenuItem";
            this.duplicateMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateMenuItem.Size = new System.Drawing.Size(244, 22);
            this.duplicateMenuItem.Text = "Duplicate";
            this.duplicateMenuItem.Click += new System.EventHandler(this.SelectionDuplicate);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteMenuItem.Size = new System.Drawing.Size(244, 22);
            this.deleteMenuItem.Text = "Delete";
            this.deleteMenuItem.Click += new System.EventHandler(this.SelectionDelete);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(241, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.DocumentSelectAll);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.selectNoneToolStripMenuItem.Text = "Select None";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.DocumentSelectNone);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(241, 6);
            // 
            // editInPlaceToolStripMenuItem
            // 
            this.editInPlaceToolStripMenuItem.Name = "editInPlaceToolStripMenuItem";
            this.editInPlaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.editInPlaceToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.editInPlaceToolStripMenuItem.Text = "Edit In Place";
            this.editInPlaceToolStripMenuItem.Click += new System.EventHandler(this.DocumentEditInPlace);
            // 
            // finishEditInPlaceToolStripMenuItem
            // 
            this.finishEditInPlaceToolStripMenuItem.Name = "finishEditInPlaceToolStripMenuItem";
            this.finishEditInPlaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.finishEditInPlaceToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.finishEditInPlaceToolStripMenuItem.Text = "Finish Edit in Place";
            this.finishEditInPlaceToolStripMenuItem.Click += new System.EventHandler(this.DocumentFinishEditInPlace);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(241, 6);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.Zoom100ToolStripMenuItem,
            this.zoomToFitToolStripMenuItem,
            this.zoomToSelectionToolStripMenuItem,
            this.toolStripMenuItem11,
            this.rulersToolStripMenuItem,
            this.gridToolStripMenuItem,
            this.guidesToolStripMenuItem,
            this.objectsToolStripMenuItem,
            this.smartBondsStripMenuItem,
            this.toolStripMenuItem13,
            this.libraryViewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.propertyBarToolStripMenuItem});
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem10.Text = "View";
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl +";
            this.zoomInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.DocumentZoomIn);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl -";
            this.zoomOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemMinus)));
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.DocumentZoomOut);
            // 
            // Zoom100ToolStripMenuItem
            // 
            this.Zoom100ToolStripMenuItem.Name = "Zoom100ToolStripMenuItem";
            this.Zoom100ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.Zoom100ToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.Zoom100ToolStripMenuItem.Text = "100%";
            this.Zoom100ToolStripMenuItem.Click += new System.EventHandler(this.DocumentZoom100Percent);
            // 
            // zoomToFitToolStripMenuItem
            // 
            this.zoomToFitToolStripMenuItem.Name = "zoomToFitToolStripMenuItem";
            this.zoomToFitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.zoomToFitToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.zoomToFitToolStripMenuItem.Text = "Zoom To Fit";
            this.zoomToFitToolStripMenuItem.Click += new System.EventHandler(this.DocumentZoomToFit);
            // 
            // zoomToSelectionToolStripMenuItem
            // 
            this.zoomToSelectionToolStripMenuItem.Name = "zoomToSelectionToolStripMenuItem";
            this.zoomToSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.zoomToSelectionToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.zoomToSelectionToolStripMenuItem.Text = "Zoom To Selection";
            this.zoomToSelectionToolStripMenuItem.Click += new System.EventHandler(this.DocumentZoomToSelection);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(232, 6);
            // 
            // rulersToolStripMenuItem
            // 
            this.rulersToolStripMenuItem.Checked = true;
            this.rulersToolStripMenuItem.CheckOnClick = true;
            this.rulersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rulersToolStripMenuItem.Name = "rulersToolStripMenuItem";
            this.rulersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
            this.rulersToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.rulersToolStripMenuItem.Text = "Show Rulers";
            this.rulersToolStripMenuItem.Click += new System.EventHandler(this.ShowRulers);
            // 
            // gridToolStripMenuItem
            // 
            this.gridToolStripMenuItem.CheckOnClick = true;
            this.gridToolStripMenuItem.Enabled = false;
            this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
            this.gridToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.gridToolStripMenuItem.Text = "Show Grid";
            this.gridToolStripMenuItem.Click += new System.EventHandler(this.ShowGrid);
            // 
            // guidesToolStripMenuItem
            // 
            this.guidesToolStripMenuItem.Checked = true;
            this.guidesToolStripMenuItem.CheckOnClick = true;
            this.guidesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.guidesToolStripMenuItem.Name = "guidesToolStripMenuItem";
            this.guidesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
            this.guidesToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.guidesToolStripMenuItem.Text = "Show Guides";
            this.guidesToolStripMenuItem.Click += new System.EventHandler(this.ShowGuides);
            // 
            // objectsToolStripMenuItem
            // 
            this.objectsToolStripMenuItem.Checked = true;
            this.objectsToolStripMenuItem.CheckOnClick = true;
            this.objectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.objectsToolStripMenuItem.Name = "objectsToolStripMenuItem";
            this.objectsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.objectsToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.objectsToolStripMenuItem.Text = "Snap To Objects";
            this.objectsToolStripMenuItem.Click += new System.EventHandler(this.SnapToObjects);
            // 
            // smartBondsStripMenuItem
            // 
            this.smartBondsStripMenuItem.CheckOnClick = true;
            this.smartBondsStripMenuItem.Name = "smartBondsStripMenuItem";
            this.smartBondsStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.smartBondsStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.smartBondsStripMenuItem.Text = "Smart Bonds";
            this.smartBondsStripMenuItem.Click += new System.EventHandler(this.UseSmartBonds);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(232, 6);
            // 
            // libraryViewToolStripMenuItem
            // 
            this.libraryViewToolStripMenuItem.Name = "libraryViewToolStripMenuItem";
            this.libraryViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.libraryViewToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.libraryViewToolStripMenuItem.Text = "Library View";
            this.libraryViewToolStripMenuItem.Click += new System.EventHandler(this.ShowLibraryView);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Enabled = false;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // propertyBarToolStripMenuItem
            // 
            this.propertyBarToolStripMenuItem.Name = "propertyBarToolStripMenuItem";
            this.propertyBarToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.propertyBarToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.propertyBarToolStripMenuItem.Text = "Property Bar";
            this.propertyBarToolStripMenuItem.Click += new System.EventHandler(this.ShowPropertyBar);
            // 
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transformToolStripMenuItem,
            this.toolStripMenuItem14,
            this.alignDistributeToolStripMenuItem,
            this.orderToolStripMenuItem,
            this.toolStripMenuItem15,
            this.groupToolStripMenuItem,
            this.ungroupToolStripMenuItem,
            this.makeContainerToolStripMenuItem,
            this.breakApartToolStripMenuItem,
            this.toolStripMenuItem16,
            this.swapContainerToolStripMenuItem,
            this.duplicateContainerToolStripMenuItem,
            this.toolStripMenuItem19,
            this.appPropertiesToolStripMenuItem});
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.commandsToolStripMenuItem.Text = "Modify";
            // 
            // transformToolStripMenuItem
            // 
            this.transformToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.numericTransformToolStripMenuItem,
            this.flipHorizontalToolStripMenuItem,
            this.flipVerticalToolStripMenuItem,
            this.rotateRight15ToolStripMenuItem,
            this.rotateLeft15ToolStripMenuItem,
            this.toolStripMenuItem17,
            this.removeTransformToolStripMenuItem});
            this.transformToolStripMenuItem.Name = "transformToolStripMenuItem";
            this.transformToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.transformToolStripMenuItem.Text = "Transform";
            // 
            // numericTransformToolStripMenuItem
            // 
            this.numericTransformToolStripMenuItem.Name = "numericTransformToolStripMenuItem";
            this.numericTransformToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.numericTransformToolStripMenuItem.Text = "Numeric Transform...";
            this.numericTransformToolStripMenuItem.Click += new System.EventHandler(this.SelectionNumericTransform);
            // 
            // flipHorizontalToolStripMenuItem
            // 
            this.flipHorizontalToolStripMenuItem.Name = "flipHorizontalToolStripMenuItem";
            this.flipHorizontalToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+/";
            this.flipHorizontalToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemQuestion)));
            this.flipHorizontalToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.flipHorizontalToolStripMenuItem.Text = "Flip Horizontal";
            this.flipHorizontalToolStripMenuItem.Click += new System.EventHandler(this.SelectionFlipHorizontal);
            // 
            // flipVerticalToolStripMenuItem
            // 
            this.flipVerticalToolStripMenuItem.Name = "flipVerticalToolStripMenuItem";
            this.flipVerticalToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+/";
            this.flipVerticalToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.OemQuestion)));
            this.flipVerticalToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.flipVerticalToolStripMenuItem.Text = "Flip Vertical";
            this.flipVerticalToolStripMenuItem.Click += new System.EventHandler(this.SelectionFlipVertical);
            // 
            // rotateRight15ToolStripMenuItem
            // 
            this.rotateRight15ToolStripMenuItem.Name = "rotateRight15ToolStripMenuItem";
            this.rotateRight15ToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+>";
            this.rotateRight15ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemPeriod)));
            this.rotateRight15ToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.rotateRight15ToolStripMenuItem.Text = "Rotate Right 15°";
            this.rotateRight15ToolStripMenuItem.Click += new System.EventHandler(this.SelectionRotateRight15Degrees);
            // 
            // rotateLeft15ToolStripMenuItem
            // 
            this.rotateLeft15ToolStripMenuItem.Name = "rotateLeft15ToolStripMenuItem";
            this.rotateLeft15ToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+<";
            this.rotateLeft15ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemcomma)));
            this.rotateLeft15ToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.rotateLeft15ToolStripMenuItem.Text = "Rotate Left 15°";
            this.rotateLeft15ToolStripMenuItem.Click += new System.EventHandler(this.SelectionRotateLeft15Degrees);
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            this.toolStripMenuItem17.Size = new System.Drawing.Size(245, 6);
            // 
            // removeTransformToolStripMenuItem
            // 
            this.removeTransformToolStripMenuItem.Name = "removeTransformToolStripMenuItem";
            this.removeTransformToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.removeTransformToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.removeTransformToolStripMenuItem.Text = "Remove Transform";
            this.removeTransformToolStripMenuItem.Click += new System.EventHandler(this.SelectionRemoveTransform);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(201, 6);
            // 
            // alignDistributeToolStripMenuItem
            // 
            this.alignDistributeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alignToLeftToolStripMenuItem,
            this.alignToRightToolStripMenuItem,
            this.alignToTopToolStripMenuItem,
            this.alignToBottomToolStripMenuItem,
            this.alignHorizontalCentersToolStripMenuItem,
            this.alignVerticalCentersToolStripMenuItem,
            this.toolStripMenuItem6,
            this.distributeToLastAlignToolStripMenuItem,
            this.toolStripMenuItem20,
            this.removeAllBondsToolStripMenuItem});
            this.alignDistributeToolStripMenuItem.Name = "alignDistributeToolStripMenuItem";
            this.alignDistributeToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.alignDistributeToolStripMenuItem.Text = "Align";
            // 
            // alignToLeftToolStripMenuItem
            // 
            this.alignToLeftToolStripMenuItem.Name = "alignToLeftToolStripMenuItem";
            this.alignToLeftToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 4";
            this.alignToLeftToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad4)));
            this.alignToLeftToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.alignToLeftToolStripMenuItem.Text = "To Left";
            this.alignToLeftToolStripMenuItem.Click += new System.EventHandler(this.SelectionAlignToLeft);
            // 
            // alignToRightToolStripMenuItem
            // 
            this.alignToRightToolStripMenuItem.Name = "alignToRightToolStripMenuItem";
            this.alignToRightToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 6";
            this.alignToRightToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad6)));
            this.alignToRightToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.alignToRightToolStripMenuItem.Text = "To Right";
            this.alignToRightToolStripMenuItem.Click += new System.EventHandler(this.SelectionAlignToRight);
            // 
            // alignToTopToolStripMenuItem
            // 
            this.alignToTopToolStripMenuItem.Name = "alignToTopToolStripMenuItem";
            this.alignToTopToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 8";
            this.alignToTopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad8)));
            this.alignToTopToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.alignToTopToolStripMenuItem.Text = "To Top";
            this.alignToTopToolStripMenuItem.Click += new System.EventHandler(this.SelectionAlignToTop);
            // 
            // alignToBottomToolStripMenuItem
            // 
            this.alignToBottomToolStripMenuItem.Name = "alignToBottomToolStripMenuItem";
            this.alignToBottomToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 2";
            this.alignToBottomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad2)));
            this.alignToBottomToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.alignToBottomToolStripMenuItem.Text = "To Bottom";
            this.alignToBottomToolStripMenuItem.Click += new System.EventHandler(this.SelectionAlignToBottom);
            // 
            // alignHorizontalCentersToolStripMenuItem
            // 
            this.alignHorizontalCentersToolStripMenuItem.Name = "alignHorizontalCentersToolStripMenuItem";
            this.alignHorizontalCentersToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 9";
            this.alignHorizontalCentersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad9)));
            this.alignHorizontalCentersToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.alignHorizontalCentersToolStripMenuItem.Text = "Horizontal Centers";
            this.alignHorizontalCentersToolStripMenuItem.Click += new System.EventHandler(this.SelectionAlignHorizontalCenters);
            // 
            // alignVerticalCentersToolStripMenuItem
            // 
            this.alignVerticalCentersToolStripMenuItem.Name = "alignVerticalCentersToolStripMenuItem";
            this.alignVerticalCentersToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 1";
            this.alignVerticalCentersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad1)));
            this.alignVerticalCentersToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.alignVerticalCentersToolStripMenuItem.Text = "Vertical Centers";
            this.alignVerticalCentersToolStripMenuItem.Click += new System.EventHandler(this.SelectionAlignVerticalCenters);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(257, 6);
            // 
            // distributeToLastAlignToolStripMenuItem
            // 
            this.distributeToLastAlignToolStripMenuItem.Name = "distributeToLastAlignToolStripMenuItem";
            this.distributeToLastAlignToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 5";
            this.distributeToLastAlignToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad5)));
            this.distributeToLastAlignToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.distributeToLastAlignToolStripMenuItem.Text = "Distribute To Last Align";
            this.distributeToLastAlignToolStripMenuItem.Click += new System.EventHandler(this.SelectionDistributeToLastAlign);
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            this.toolStripMenuItem20.Size = new System.Drawing.Size(257, 6);
            // 
            // removeAllBondsToolStripMenuItem
            // 
            this.removeAllBondsToolStripMenuItem.Name = "removeAllBondsToolStripMenuItem";
            this.removeAllBondsToolStripMenuItem.ShortcutKeyDisplayString = "NumPad 0";
            this.removeAllBondsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad0)));
            this.removeAllBondsToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.removeAllBondsToolStripMenuItem.Text = "Remove All Bonds";
            this.removeAllBondsToolStripMenuItem.Click += new System.EventHandler(this.SelectionRemoveAllBonds);
            // 
            // orderToolStripMenuItem
            // 
            this.orderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toTopToolStripMenuItem,
            this.toBottomToolStripMenuItem,
            this.upOneToolStripMenuItem,
            this.backOneToolStripMenuItem,
            this.infrontOfToolStripMenuItem,
            this.behindToolStripMenuItem});
            this.orderToolStripMenuItem.Name = "orderToolStripMenuItem";
            this.orderToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.orderToolStripMenuItem.Text = "Order";
            // 
            // toTopToolStripMenuItem
            // 
            this.toTopToolStripMenuItem.Name = "toTopToolStripMenuItem";
            this.toTopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Up)));
            this.toTopToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.toTopToolStripMenuItem.Text = "To Top";
            this.toTopToolStripMenuItem.Click += new System.EventHandler(this.SelectionToTop);
            // 
            // toBottomToolStripMenuItem
            // 
            this.toBottomToolStripMenuItem.Name = "toBottomToolStripMenuItem";
            this.toBottomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Down)));
            this.toBottomToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.toBottomToolStripMenuItem.Text = "To Bottom";
            this.toBottomToolStripMenuItem.Click += new System.EventHandler(this.SelectionToBottom);
            // 
            // upOneToolStripMenuItem
            // 
            this.upOneToolStripMenuItem.Name = "upOneToolStripMenuItem";
            this.upOneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.upOneToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.upOneToolStripMenuItem.Text = "Up One";
            this.upOneToolStripMenuItem.Click += new System.EventHandler(this.SelectionUpOne);
            // 
            // backOneToolStripMenuItem
            // 
            this.backOneToolStripMenuItem.Name = "backOneToolStripMenuItem";
            this.backOneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.backOneToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.backOneToolStripMenuItem.Text = "Back One";
            this.backOneToolStripMenuItem.Click += new System.EventHandler(this.SelectionBackOne);
            // 
            // infrontOfToolStripMenuItem
            // 
            this.infrontOfToolStripMenuItem.Enabled = false;
            this.infrontOfToolStripMenuItem.Name = "infrontOfToolStripMenuItem";
            this.infrontOfToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.infrontOfToolStripMenuItem.Text = "Infront Of...";
            this.infrontOfToolStripMenuItem.Click += new System.EventHandler(this.SelectionInfrontOf);
            // 
            // behindToolStripMenuItem
            // 
            this.behindToolStripMenuItem.Enabled = false;
            this.behindToolStripMenuItem.Name = "behindToolStripMenuItem";
            this.behindToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.behindToolStripMenuItem.Text = "Behind...";
            this.behindToolStripMenuItem.Click += new System.EventHandler(this.SelectionBehind);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(201, 6);
            // 
            // groupToolStripMenuItem
            // 
            this.groupToolStripMenuItem.Enabled = false;
            this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
            this.groupToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.groupToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.groupToolStripMenuItem.Text = "Group";
            this.groupToolStripMenuItem.Click += new System.EventHandler(this.SelectionGroup);
            // 
            // ungroupToolStripMenuItem
            // 
            this.ungroupToolStripMenuItem.Enabled = false;
            this.ungroupToolStripMenuItem.Name = "ungroupToolStripMenuItem";
            this.ungroupToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.ungroupToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.ungroupToolStripMenuItem.Text = "Ungroup";
            this.ungroupToolStripMenuItem.Click += new System.EventHandler(this.SelectionUngroup);
            // 
            // makeContainerToolStripMenuItem
            // 
            this.makeContainerToolStripMenuItem.Name = "makeContainerToolStripMenuItem";
            this.makeContainerToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.makeContainerToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.makeContainerToolStripMenuItem.Text = "Make Container";
            this.makeContainerToolStripMenuItem.Click += new System.EventHandler(this.SelectionMakeContainer);
            // 
            // breakApartToolStripMenuItem
            // 
            this.breakApartToolStripMenuItem.Name = "breakApartToolStripMenuItem";
            this.breakApartToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.breakApartToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.breakApartToolStripMenuItem.Text = "Break Apart";
            this.breakApartToolStripMenuItem.Click += new System.EventHandler(this.SelectionBreakApart);
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(201, 6);
            // 
            // swapContainerToolStripMenuItem
            // 
            this.swapContainerToolStripMenuItem.Enabled = false;
            this.swapContainerToolStripMenuItem.Name = "swapContainerToolStripMenuItem";
            this.swapContainerToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.swapContainerToolStripMenuItem.Text = "Swap Object";
            this.swapContainerToolStripMenuItem.Click += new System.EventHandler(this.SelectionSwapObject);
            // 
            // duplicateContainerToolStripMenuItem
            // 
            this.duplicateContainerToolStripMenuItem.Name = "duplicateContainerToolStripMenuItem";
            this.duplicateContainerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateContainerToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.duplicateContainerToolStripMenuItem.Text = "Duplicate Object";
            this.duplicateContainerToolStripMenuItem.Click += new System.EventHandler(this.SelectionDuplicateObject);
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.Size = new System.Drawing.Size(201, 6);
            // 
            // appPropertiesToolStripMenuItem
            // 
            this.appPropertiesToolStripMenuItem.Enabled = false;
            this.appPropertiesToolStripMenuItem.Name = "appPropertiesToolStripMenuItem";
            this.appPropertiesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.appPropertiesToolStripMenuItem.Text = "App Properties";
            this.appPropertiesToolStripMenuItem.Click += new System.EventHandler(this.ShowDocumentProperties);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onlineHelpToolStripMenuItem,
            this.shortcutKeysToolStripMenuItem,
            this.toolStripMenuItem18,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // onlineHelpToolStripMenuItem
            // 
            this.onlineHelpToolStripMenuItem.Name = "onlineHelpToolStripMenuItem";
            this.onlineHelpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.onlineHelpToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.onlineHelpToolStripMenuItem.Text = "Help";
            this.onlineHelpToolStripMenuItem.Click += new System.EventHandler(this.OnlineHelp);
            // 
            // shortcutKeysToolStripMenuItem
            // 
            this.shortcutKeysToolStripMenuItem.Name = "shortcutKeysToolStripMenuItem";
            this.shortcutKeysToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.shortcutKeysToolStripMenuItem.Text = "Shortcut Keys";
            this.shortcutKeysToolStripMenuItem.Click += new System.EventHandler(this.ShowShortcutKeysHelp);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            this.toolStripMenuItem18.Size = new System.Drawing.Size(143, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.ShowAbout);
            // 
            // levelBar
            // 
            this.levelBar.Location = new System.Drawing.Point(0, 24);
            this.levelBar.Name = "levelBar";
            this.levelBar.Size = new System.Drawing.Size(1234, 25);
            this.levelBar.TabIndex = 1;
            this.levelBar.Text = "Level Bar";
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.AllowDrop = true;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.SystemColors.Control;
            this.dockPanel.Location = new System.Drawing.Point(0, 49);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1234, 568);
            dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = System.Drawing.SystemColors.Control;
            tabGradient1.StartColor = System.Drawing.SystemColors.Control;
            tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            autoHideStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            dockPaneStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = System.Drawing.SystemColors.Control;
            tabGradient5.StartColor = System.Drawing.SystemColors.Control;
            tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = System.Drawing.SystemColors.InactiveCaption;
            tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = System.Drawing.SystemColors.InactiveCaptionText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = System.Drawing.Color.Transparent;
            tabGradient7.StartColor = System.Drawing.Color.Transparent;
            tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            this.dockPanel.Skin = dockPanelSkin1;
            this.dockPanel.TabIndex = 3;
            // 
            // elementIcons
            // 
            this.elementIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("elementIcons.ImageStream")));
            this.elementIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.elementIcons.Images.SetKeyName(0, "imageIcon.bmp");
            this.elementIcons.Images.SetKeyName(1, "folderClosed.png");
            this.elementIcons.Images.SetKeyName(2, "folderOpen.png");
            this.elementIcons.Images.SetKeyName(3, "folderFlashClosed.png");
            this.elementIcons.Images.SetKeyName(4, "folderFlashOpen.png");
            this.elementIcons.Images.SetKeyName(5, "vectorSymbol.png");
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 617);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.levelBar);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "UI Layout Tool - Test";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStrip levelBar;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem duplicateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem editInPlaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Zoom100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToFitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem rulersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guidesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem transformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numericTransformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipHorizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipVerticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateRight15ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateLeft15ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem17;
        private System.Windows.Forms.ToolStripMenuItem removeTransformToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem alignDistributeToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem alignToLeftToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem alignToRightToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem alignToTopToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem alignToBottomToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem alignHorizontalCentersToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem alignVerticalCentersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infrontOfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem behindToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem groupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ungroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeContainerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem breakApartToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem16;
        private System.Windows.Forms.ToolStripMenuItem swapContainerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateContainerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem19;
        private System.Windows.Forms.ToolStripMenuItem appPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlineHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem18;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem finishEditInPlaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        public System.Windows.Forms.ToolStripMenuItem distributeToLastAlignToolStripMenuItem;
        private System.Windows.Forms.ImageList elementIcons;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSVGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportXAMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDirectXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAndroidToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportIOSToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exportCanvasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem libraryViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertyBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smartBondsStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem20;
        private System.Windows.Forms.ToolStripMenuItem removeAllBondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInExplorerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWorkingFolderMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem shortcutKeysToolStripMenuItem;

    }
}

