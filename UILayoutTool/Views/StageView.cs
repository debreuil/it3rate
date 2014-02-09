using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using DDW.Assets;
using DDW.Display;
using System.Collections.Generic;
using DDW.Interfaces;
using DDW.Commands;
using Vex = DDW.Vex;
using DDW.Managers;
using DDW.Enums;
using DDW.Utils;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DDW.Controls;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Xml;
using System.Globalization;
using DDW.Gdi;
using DDW.Vex.Bonds;
using DDW.Data;

namespace DDW.Views
{
    public class StageView : Form, IEditableView
    {
        bool dragInCursor = false;

        #region fields/properties

        public Vex.VexObject vexObject;
        private static Vex.Rectangle defaultPaperSize = new Vex.Rectangle(0, 0, 550, 400);
        //private static Vex.Color defaultPaperColor = new Vex.Color(255, 255, 255, 255);
        private static Vex.Color defaultPaperColor = new Vex.Color(255, 255, 255, 255);
        private static Color stageColor = Color.FromArgb(255, 230, 230, 230);
        private SolidBrush paperColor;

        public Gdi.GdiRenderer Gdi;
        public BitmapCache BitmapCache;

        public string rootFolder;
        public string fileName;
        public string FileFullPathAndName { get { return rootFolder + Path.DirectorySeparatorChar + fileName; } }

        private string definitionsFolder = "_definitions";
        public string DefinitionsFolder { get { return definitionsFolder; } }
        public string DefinitionsFolderFull { get { return rootFolder + Path.DirectorySeparatorChar + Name + definitionsFolder + Path.DirectorySeparatorChar; } }
        
        private string instancesFolder = "_instances";
        public string InstancesFolder { get { return instancesFolder; } }
        public string InstancesFolderFull { get { return rootFolder + Path.DirectorySeparatorChar + Name + instancesFolder + Path.DirectorySeparatorChar; } }

        private string workingFolder = "_working";
        public string WorkingFolder { get { return workingFolder; } set { workingFolder = value; } }
        public string WorkingFolderFull { get { return rootFolder + Path.DirectorySeparatorChar + Name + workingFolder + Path.DirectorySeparatorChar; } }
       
        private string exportFolder = "_export";
        public string ExportFolder { get { return exportFolder; } set { exportFolder = value; } }
        public string ExportFolderFull { get { return rootFolder + Path.DirectorySeparatorChar + Name + exportFolder + Path.DirectorySeparatorChar; } }
        
        private string versionNumber = "0.0.1";
        public string VersionNumber { get { return versionNumber; } }

        private Library library;
        public Library Library { get { return library; } }
        private InstanceManager instanceManager;
        public InstanceManager InstanceManager { get { return instanceManager; } }
        

        private const float MAX_SCALE = 100;
        private const float MIN_SCALE = 0.1f;

        public DesignTimeline Root { get { return root; } }
        private DesignTimeline root;
        private DesignTimeline designStage;
        public uint RootId = 1;
        public bool isNewFile = true;

        public bool HasEditFocus()
        {
            return this.Focused;
        }

        private bool hasSaveableChanges = false;
        public bool HasSaveableChanges
        {
            get
            {
                return isNewFile || hasSaveableChanges || instanceManager.HasSaveableChanges || library.HasSaveableChanges;
            }
            set
            {
                hasSaveableChanges = value;
                designStage.Instance.HasSaveableChanges = value;
                this.Text = value ? Name + "*" : Name;
            }
        }
        private Stack<DesignTimeline> editStack = new Stack<DesignTimeline>();
        public Stack<DesignTimeline> EditStack { get { return editStack; } }

        private CommandStack commandStack = new CommandStack();
        public CommandStack CommandStack { get { return commandStack; } }

        private TransformHandles transformHandles;
        private Rulers rulers;
        private Rectangle localRulerSelection;
        private Vex.Point[] snapPoints = new Vex.Point[9];
        private BondAttachment[] snapTypes = new BondAttachment[9];

        private FastBitmap clickMap;
        private RectangleF prevVisibleClipBounds;
        public Size CanvasSize { get { return Size.Ceiling(pb.Size); } }
        Point mouseDownOriginCamera;
        Point mouseDownOriginStage;
        Point mouseCurrentCamera;
        uint clickedColor;
        bool isMouseDown;
        bool cancelMouseUp = false;
        bool clickMapNeedsUpdate = false;
        Keys prevModifierKeys = Keys.None;

        ColorMask draggingElement = ColorMask.Last;
        Rectangle prevInvalidateRect;
        Rectangle dragRect;
        Rectangle prevDragRect;
        PointF prevCenterPoint;
        Rectangle prevTransformRect;

        Pen wideWhitePen = new Pen(Color.White, 4);
        Pen allowedPen = new Pen(Color.LimeGreen, 3);
        Pen notAllowedPen = new Pen(Color.Red, 3);
        Pen selectionRectPen = new Pen(Color.Black, 1);
        Pen invalidDebugPen = new Pen(Color.YellowGreen, 1);
        private Bitmap screenCaptureCache;
        //private Bitmap selectionImageCache;
        private int nudge = 1;
        private int superNudge = 5;
        private bool selectionNeedsInvalidate;
        private bool centerNeedsInvalidate;
        private bool wasTransformed;
        private PictureBox pb;

        private bool isDebugMode;
        private bool isNameMode;


        public DesignTimeline CurrentEditItem { get { return designStage; } }
        public InstanceGroup Selection { get { return (designStage != null) ? designStage.Selected : null; } }
        public int InstanceCount { get { return (designStage != null) ? designStage.InstanceIds.Length : 0; } }
        Matrix identityMatrix = new Matrix();
        private Matrix cameraMatrix;
        public Matrix CameraMatrix
        {
            get
            {
                return cameraMatrix;
            }
        }
        public Vex.Color BackgroundColor
        {
            get { return vexObject.BackgroundColor; } 
            set
            {
                vexObject.BackgroundColor = value; 
                paperColor = new SolidBrush(vexObject.BackgroundColor.SysColor());
            }
        }
        private float zoomTickSize = 1f / 16f;

        private bool showRulers = true;
        private bool showGrid = false;
        private bool showGuides = true;
        private bool snapToObjects = true;
        public bool ShowRulers { get { return showRulers; } set { showRulers = value; pb.Invalidate(); } }
        public bool ShowGrid { get { return showGrid; } set { showGrid = value; pb.Invalidate(); } }
        public bool ShowGuides { get { return showGuides; } set { showGuides = value; pb.Invalidate(); } }
        public bool SnapToObjects { get { return snapToObjects; } set { snapToObjects = value; pb.Invalidate(); } }
        public bool UseSmartBonds { get; set; }

        #endregion

        #region Events
        public delegate void CountHandler(StageView obj, int count);
        public event CountHandler OnSelectionChanged;
        public event EventHandler OnUndoStackChanged;
        public event EventHandler OnNestedEditChanged;
        public event EventHandler OnDepthChanged;
        #endregion

        public StageView(Library library, InstanceManager instanceManager, string name)
        {
            this.library = library;
            this.instanceManager = instanceManager;
            this.Name = name;
            this.Text = name;

            vexObject = new Vex.VexObject(name);
            vexObject.ViewPort = defaultPaperSize;
            vexObject.BackgroundColor = defaultPaperColor;
            paperColor = new SolidBrush(vexObject.BackgroundColor.SysColor());

            library.stage = this;
            instanceManager.stage = this;
            BitmapCache = new Managers.BitmapCache(this);

            Gdi = new DDW.Gdi.GdiRenderer(this);
            
            InitializeComponent();

            commandStack.OnUndoStackChanged += UndoStackChanged;
        }

        public void MarkAllHasSaveableChanges(bool val)
        {
            HasSaveableChanges = val;
            instanceManager.HasSaveableChanges = val;
            library.HasSaveableChanges = val;
        }

        public bool IsEditingRoot { get { return designStage == root; } }
        public void CreateRoot()
        {
            DDW.Display.DesignTimeline newRoot = CreateEmptyInstance();
            SetRoot(newRoot);
            ResetZoom();

            instanceManager.HasSaveableChanges = false;
            newRoot.HasSaveableChanges = false;

            library[newRoot.DefinitionId].HasSaveableChanges = false;
            library.HasSaveableChanges = false;
            HasSaveableChanges = false;
        }
        public void SetRoot(DDW.Display.DesignTimeline newRoot)
        {
            if (root != null)
            {
                root.Selected.ContentsChanged -= SelectionChanged;
            }

            vexObject.Root = (Vex.Timeline)newRoot.Definition;
            root = newRoot;
            root.isRoot = true;
            designStage = newRoot;

            editStack.Clear();
            editStack.Push(root);

            cameraMatrix = new Matrix();

            transformHandles = new TransformHandles(this);
            rulers = new Rulers(this);

            pb.Paint -= new PaintEventHandler(OnPBPaint);
            pb.Paint += new PaintEventHandler(OnPBPaint);

            AddEvents();

            root.EnsureSnaps();

            OnSelectionChanged(this, 0);
            OnUndoStackChanged(this, EventArgs.Empty);
            OnNestedEditChanged(this, EventArgs.Empty);
        }
        private void AddEvents()
        {
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.StageView_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.StageView_DragEnter);
            this.DragOver += new DragEventHandler(StageView_DragOver);
            this.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.StageView_GiveFeedback);

            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.StageView_MouseWheel);
            this.pb.DoubleClick += new EventHandler(this.pb_DoubleClick);
            this.pb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StageView_MouseDown);
            this.pb.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StageView_MouseMove);
            this.pb.MouseUp += new System.Windows.Forms.MouseEventHandler(this.StageView_MouseUp);

            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.StageView_PreviewKeyDown);

            this.designStage.Selected.ContentsChanged += SelectionChanged;
        }
        private void RemoveEvents()
        {
            this.DragEnter -= new System.Windows.Forms.DragEventHandler(this.StageView_DragEnter);
            this.DragOver -= new DragEventHandler(StageView_DragOver);
            this.DragDrop -= new System.Windows.Forms.DragEventHandler(this.StageView_DragDrop);
            this.GiveFeedback -= new System.Windows.Forms.GiveFeedbackEventHandler(this.StageView_GiveFeedback);

            this.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.StageView_MouseWheel);
            this.pb.DoubleClick -= new EventHandler(this.pb_DoubleClick);
            this.pb.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.StageView_MouseDown);
            this.pb.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.StageView_MouseMove);
            this.pb.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.StageView_MouseUp);

            this.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.StageView_PreviewKeyDown);

            if (designStage != null)
            {
                this.designStage.Selected.ContentsChanged -= SelectionChanged;
            }
        }

        #region init
        private void InitializeComponent()
        {
            this.pb = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.SuspendLayout();
            // 
            // pb
            // 
            this.pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb.Location = new System.Drawing.Point(0, 0);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(489, 449);
            this.pb.TabIndex = 0;
            this.pb.TabStop = false;
            // 
            // StageView
            // 
            this.AllowDrop = true;
            this.ClientSize = new System.Drawing.Size(489, 449);
            this.Controls.Add(this.pb);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "StageView";
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.ResumeLayout(false);

        }

        public void ResetTransformHandles()
        {
            transformHandles.Reset();
        }

        public PointF CameraToStage(PointF p)
        {
            return GdiRenderer.GetTransformedPoint(p, cameraMatrix, true);
        }
        public RectangleF CameraToStage(RectangleF r)
        {
            return GdiRenderer.GetTransformedBounds(r, cameraMatrix, true);
        }
        public PointF StageToCamera(PointF p)
        {
            return GdiRenderer.GetTransformedPoint(p, cameraMatrix, false);
        }
        public RectangleF StageToCamera(RectangleF r)
        {
            return GdiRenderer.GetTransformedBounds(r, cameraMatrix, false);
        }

        public PointF RotationCenterToCamera
        {
            get
            {
                return StageToCamera(Selection.GlobalRotationCenter.SysPointF());
            }
        }
        public PointF SelectionCenterToCamera
        {
            get
            {
                return StageToCamera(Selection.SelectionCenter.SysPointF());
            }
        }
        public PointF[] TransformPointsCamera
        {
            get
            {
                return Selection.GetTransformedPoints(cameraMatrix);
            }
        }
        #endregion

        #region Commands
        public void Undo()
        {
            selectionNeedsInvalidate = commandStack.Undo();
            InvalidateSelection();
        }
        public void Redo()
        {
            selectionNeedsInvalidate = commandStack.Redo();
            InvalidateSelection();
        }
        public void Repeat()
        {
            if (CommandStack.Peek() is RepeatCommand)
            {
                RepeatCommand rc = (RepeatCommand)CommandStack.Peek();
                CommandStack.Do(rc.GetRepeatCommand());
            }
            else
            {
                CommandStack.Do(new RepeatCommand());
            }
            InvalidateSelection();
        }
        public bool CanUndo()
        {
            return commandStack.CanUndo();
        }
        public bool CanRedo()
        {
            return commandStack.CanRedo();
        }
        public void Delete()
        {
            CommandStack.Do(new DeleteInstancesCommand(Selection.SelectedIds));
            InvalidateSelection();
        }
        public void Duplicate()
        {
            Vex.Point duplicateOffset = new Vex.Point(0, 0);
            CommandStack.Do(new DuplicateSelectedCommand(duplicateOffset));
            InvalidateSelection();
        }
        public void SelectAll()
        {
            CommandStack.Do(new SelectInstancesCommand(designStage.InstanceIds, SelectionModifier.SetSelection));
            InvalidateSelection();
        }
        public void SelectNone()
        {
            CommandStack.Do(new SelectInstancesCommand(new uint[]{}, SelectionModifier.SetSelection));
            InvalidateSelection();
        }
        public void EditInPlace()
        {
            if (CanEditInPlace())
            {
                commandStack.Do(new EditInPlaceCommand(Selection.SelectedIds[0]));
                cancelMouseUp = true;
            }
        }
        public bool CanEditInPlace()
        {
            return (Selection.SelectedIds.Length == 1);
        }
        public void FinishEditInPlace(int popCount)
        {
            if (CanFinishEditInPlace(popCount))
            {
                commandStack.Do(new EditInPlaceCommand(popCount));
                cancelMouseUp = true;
            }
        }
        public bool CanFinishEditInPlace(int popCount)
        {
            return (editStack.Count > popCount);
        }
        public void ZoomSelection(int ticks)
        {
            PointF cent = pb.Bounds.Center();
            if (Selection.Count > 0)
            {
                PointF selCent = SelectionCenterToCamera;
                cameraMatrix.Translate(cent.X - selCent.X, cent.Y - selCent.Y, MatrixOrder.Append);
                //Zoom(ticks, SelectionCenterToCamera);
            }
            Zoom(ticks, cent);

        }
        public void Zoom(int ticks, PointF zoomLocation)
        {
            float maxScale = Math.Max(cameraMatrix.Elements[0], cameraMatrix.Elements[3]);
            float minScale = Math.Min(cameraMatrix.Elements[0], cameraMatrix.Elements[3]);

            float change = Math.Min(Math.Max(ticks, -5), 5) * zoomTickSize;

            float scale = change + 1;

            if (maxScale * scale > MAX_SCALE)
            {
                scale = MAX_SCALE / maxScale;
            }

            if (minScale * scale < MIN_SCALE)
            {
                scale = MIN_SCALE / minScale;
            }

            if (scale > 1.001 || scale < .999)
            {
                cameraMatrix.ScaleAt(scale, scale, zoomLocation);
                InvalidateSelection();
                pb.Invalidate();
            }
        }
        public void ResetZoom()
        {
            if (CanResetZoom())
            {
                cameraMatrix.Dispose();

                Size paperSize = vexObject.ViewPort.Size.SysSize();
                Rectangle pbRect = pb.ClientRectangle;
                float wOffset = (pbRect.Width - paperSize.Width) / 2;
                float hOffset = (pbRect.Height - paperSize.Height) / 2;

                cameraMatrix = new Matrix(1,0,0,1,wOffset, hOffset);
                pb.Invalidate();
            }
        }
        public void ZoomToFit()
        {
            Vex.Point loc;
            Vex.Rectangle bounds = Selection.CalculateBounds(designStage.InstanceIds, out loc);

            PointF[] pts = bounds.SysPointFs();
            cameraMatrix.TransformPoints(pts);
            ZoomToRect(pts.GetBounds());
        }
        public void ZoomToSelection()
        {
            ZoomToRect(TransformPointsCamera.GetBounds());
        }
        public void ZoomToRect(Rectangle userRect)
        {
            Rectangle pbRect = pb.ClientRectangle;
            PointF cent = pbRect.Center();
            PointF userCenter = userRect.Center();
            cameraMatrix.Translate(cent.X - userCenter.X, cent.Y - userCenter.Y, MatrixOrder.Append);

            float xScale = pbRect.Width / (float)userRect.Width;
            float yScale = pbRect.Height / (float)userRect.Height;
            float minScale = Math.Min(xScale, yScale) * .95f;

            cameraMatrix.ScaleAt(minScale, minScale, cent);
            InvalidateSelection();
            pb.Invalidate();

        }
        public bool CanZoomIn()
        {
            float scale = Math.Max(cameraMatrix.Elements[0], cameraMatrix.Elements[3]);
            return (scale < MAX_SCALE - .001f);
        }
        public bool CanZoomOut()
        {
            float scale = Math.Min(cameraMatrix.Elements[0], cameraMatrix.Elements[3]);
            return (scale > MIN_SCALE + .001f);
        }
        public bool CanResetZoom()
        {
            return true;// !cameraMatrix.IsIdentity();
        }
        public void FlipVertical()
        {
            Vex.Point center = Selection.GetTransformedPoints().GetBounds().Center().VexPoint();
            commandStack.Do(new ScaleTransformCommand(1, -1, center));
        }
        public void FlipHorizontal()
        {
            Vex.Point center = Selection.GetTransformedPoints().GetBounds().Center().VexPoint();
            commandStack.Do(new ScaleTransformCommand(-1, 1, center));
        }
        public void Rotate(float degrees)
        {
            commandStack.Do(new RotateTransformCommand(degrees));
        }
        public void RemoveTransforms()
        {
            commandStack.Do(new RemoveTransformsCommand(Selection.SelectedIds));
        }

        public void AlignToLeft()
        {
            commandStack.Do(new AlignCommand(ChainType.AlignedLeft));
        }
        public void AlignToRight()
        {
            commandStack.Do(new AlignCommand(ChainType.AlignedRight));
        }
        public void AlignToTop()
        {
            commandStack.Do(new AlignCommand(ChainType.AlignedTop));
        }
        public void AlignToBottom()
        {
            commandStack.Do(new AlignCommand(ChainType.AlignedBottom));
        }
        public void AlignHorizontalCenter()
        {
            commandStack.Do(new AlignCommand(ChainType.AlignedCenterHorizontal));
        }
        public void AlignVerticalCenter()
        {
            commandStack.Do(new AlignCommand(ChainType.AlignedCenterVertical));
        }
        public void DistributeToLastAlign()
        {
            if (CanDistributeToLastAlign())
            {
                AlignCommand lastCommand = (AlignCommand)commandStack.Peek();
                if (lastCommand.ChainType.IsAligned())
                {
                    if (lastCommand.ChainType.IsHorizontal())
                    {
                        commandStack.Do(new AlignCommand(ChainType.DistributedHorizontal));
                    }
                    else
                    {
                        commandStack.Do(new AlignCommand(ChainType.DistributedVertical));
                    }
                }
            }
        }
        public void RemoveBondsFromSelection()
        {
            commandStack.Do(new RemoveBondsCommand());
        }

        public bool CanDistributeToLastAlign()
        {
            bool result = false;

            if (commandStack.CanUndo())
            {
                ICommand lastCommand = commandStack.Peek();
                if (lastCommand is AlignCommand)
                {
                    result = ((AlignCommand)lastCommand).ChainType.IsAligned();
                }
            }

            return result;
        }

        public void MakeContainer()
        {
            commandStack.Do(new SelectionToSymbolCommand());
        }
        public void BreakApart()
        {
            commandStack.Do(new BreakApartCommand());
            InvalidateSelection();
        }
        public bool CanBreakApart()
        {
            return BreakApartCommand.CanBreakApart(Selection.SelectedIds);
        }

        public bool ToggleNameMode()
        {
            return isNameMode = !isNameMode;
        }

        public DesignInstance AddInstance(uint libraryId, Vex.Point location)
        {
            DesignInstance di = CurrentEditItem.Add(libraryId, location);
            return di;
        }
        public DesignInstance AddInstance(UsageIdentifier uid)
        {
            uid.Parent.InsertExistingOnUndo(uid.Depth, uid.Instance);
            return uid.Instance;
        }
        public uint[] AddInstances(uint[] libraryIds, Vex.Point[] locations)
        {
            uint[] result = CurrentEditItem.AddRange(libraryIds, locations);
            return result;
        }
        public void AddInstancesById(uint[] instanceIds)
        {
            CurrentEditItem.AddInstancesById(instanceIds);
        }
        public DesignInstance[] RemoveInstancesById(uint[] instanceIds)
        {
            DesignInstance[] result = CurrentEditItem.RemoveInstancesById(instanceIds);
            return result;
        }
        public UsageIdentifier[] RemoveInstancesByIdGlobal(uint[] instanceIds)
        {
            List<UsageIdentifier> result = new List<UsageIdentifier>();
            List<uint> parentIds = new List<uint>();
            foreach (uint id in instanceIds)
            {
                DesignInstance di = instanceManager[id];
                uint parentId = di.ParentDefinition.Id;
                Vex.Timeline tl = (Vex.Timeline)library[parentId].Definition;
                tl.RemoveInstance(di.Instance);
                //DesignTimeline parentDt = (DesignTimeline)instanceManager[parentId];
                //result.Add(new UsageIdentifier(parentDt, di, di.Depth));
               // di = parentDt.Remove(id);


                //uint[] parentUsages = library.FindAllUsagesOfDefinition(di.ParentDefinition.Id);
                //// should always have at least one result
                //// removing from one will remove from all as it operates on the def.
                //if (parentUsages.Length > 0)
                //{
                //    DesignTimeline parentDt = (DesignTimeline)instanceManager[parentUsages[0]];
                //    result.Add(new UsageIdentifier(parentDt, di, di.Depth));
                //    di = parentDt.Remove(id);
                //}

                parentIds.Add(parentId);
                //LibraryItem li = library[parent.Id];
                //li.CalculateBounds();
            }

            foreach (uint id in parentIds)
            {
                if (library.Contains(id))
                {
                    library[id].CalculateBounds();
                }
            }

            return result.ToArray();
        }
        public void InsertExistingInstance(int depth, DesignInstance di)
        {
            CurrentEditItem.InsertExistingOnUndo(depth, di);
        }

        public DesignTimeline CreateEmptyInstance()
        {
            DesignTimeline result;
            Vex.Instance inst = new Vex.Instance();

            Vex.Timeline tl = new Vex.Timeline(Library.NextLibraryId());
            LibraryItem li = CreateLibraryItem(tl, true);
            inst.DefinitionId = tl.Id;
            inst.Location = Vex.Point.Zero;

            result = new DesignTimeline(this, inst);
            instanceManager.AddInstance(result);

            return result;
        }
        public DesignInstance CreateInstance(uint definitionId, Vex.Point location)
        {
            DesignInstance result;

            Vex.Instance inst = new Vex.Instance();
            inst.DefinitionId = definitionId;
            inst.Location = location;

            result = CreateInstance(inst);
            return result;
        }
        public DesignInstance CreateInstance(Vex.Instance inst)
        {
            DesignInstance result = null;

            if (Library.Contains(inst.DefinitionId)) // todo: account for missing files
            {
                if (Library[inst.DefinitionId].Definition is Vex.Timeline)
                {
                    result = new DesignTimeline(this, inst);
                }
                else
                {
                    result = new DesignInstance(this, inst);
                }
                instanceManager.AddInstance(result);
            }
            else // missing asset
            {
            }
            return result;
        }
        public LibraryItem CreateLibraryItem(Vex.IDefinition def, bool hasId)
        {
            LibraryItem li = hasId ? new LibraryItem(this, def) : new LibraryItem(this, def, library.NextLibraryId());
            library.AddLibraryItem(li);
            return li;
        }

        public void TranslateSelection(Vex.Point offset, bool doSnaps, List<Bond> addedBonds, List<Bond> previousBonds)
        {
            uint[] relIds = Selection.RelatedIds;
            for (int i = 0; i < relIds.Length; i++)
            {
                CurrentEditItem.SnapStore.RemoveInstance(instanceManager[relIds[i]]);
            }

            Selection.TranslateSelection(offset);

            for (int i = 0; i < relIds.Length; i++)
            {
                CurrentEditItem.SnapStore.AddInstance(instanceManager[relIds[i]]);
            }

            if (doSnaps)
            {
                // newBonds format: {uint: [selId, currentBA, instHash, newBA, handle index]}
                List<uint[]> newBonds = new List<uint[]>();
                if (Selection.Count == 1)
                {
                    uint selId = Selection[0];
                    for (int i = 0; i < snapPoints.Length; i++)
                    {
                        if (!float.IsNaN(snapPoints[i].X) && !float.IsNaN(snapPoints[i].Y))
                        {
                            BondAttachment curBa = BondAttachmentExtensions.GetTargetFromHandleIndex(i);
                            BondAttachment st = snapTypes[i];
                            ICrossPoint[] cps = CurrentEditItem.SnapStore.GetCrossPoint(snapPoints[i], snapTypes[i]);

                            if (cps.Length > 0 && (cps[0].InstanceHash != selId))
                            {
                                if (st.IsHandle() || st.IsGuide())
                                {
                                    newBonds.Add(new uint[] { selId, (uint)curBa, cps[0].InstanceHash, (uint)cps[0].BondAttachment, (uint)i });
                                }
                            }
                        }
                    }
                }

                foreach (uint[] ar in newBonds)
                {
                    BondAttachment ba = (BondAttachment)ar[3]; // new bondAttachment
                    Vex.Point target = snapPoints[ar[4]];      // handleIndex

                    if (ba.IsHandle())
                    {
                        CurrentEditItem.BondStore.JoinHandles(ar[0], (BondAttachment)ar[1], ar[2], (BondAttachment)ar[3], target, addedBonds, previousBonds);
                    }
                    else if (ba.IsGuide())
                    {
                        CurrentEditItem.BondStore.LockToGuide(ar[0], (BondAttachment)ar[1], ar[2], (BondAttachment)ar[3], target, addedBonds, previousBonds);
                    }
                }
            }                    

            Selection.TranslateSelection(Vex.Point.Zero); // todo: this hack basically recalculates new bonds
        }
        public void ScaleSelectionAt(float scaleX, float scaleY, Vex.Point scaleCenter)
        {
            uint[] selIds = Selection.SelectedIds;
            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.RemoveInstance(instanceManager[selIds[i]]);
            }

            Selection.ScaleAt(scaleX, scaleY, scaleCenter);

            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.AddInstance(instanceManager[selIds[i]]);
            }
        }
        public void RotateSelectionAt(float angle, Vex.Point rotateCenter)
        {
            uint[] selIds = Selection.SelectedIds;
            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.RemoveInstance(instanceManager[selIds[i]]);
            }

            Selection.RotateAt(angle, rotateCenter);

            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.AddInstance(instanceManager[selIds[i]]);
            }
        }
        public Vex.Matrix[] RemoveSelectionTransform()
        {
            uint[] selIds = Selection.SelectedIds;
            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.RemoveInstance(instanceManager[selIds[i]]);
            }

            Vex.Matrix[] prevMatrices = new Vex.Matrix[selIds.Length];
            for (int i = 0; i < selIds.Length; i++)
            {
                DesignInstance di = MainForm.CurrentInstanceManager[selIds[i]];
                prevMatrices[i] = di.GetMatrix();
                Vex.Point orgCenter = di.StrokeBounds.Center;
                Vex.Point newMidpoint = di.UntransformedBounds.Center;
                // stroke bounds adds 1 to avoid gdi drawing glitch, so subtract it
                di.SetMatrix(new Vex.Matrix(1, 0, 0, 1, orgCenter.X - 1 - newMidpoint.X, orgCenter.Y - 1 - newMidpoint.Y));
            }
            Selection.Update();

            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.AddInstance(instanceManager[selIds[i]]);
            }

            ResetTransformHandles();
            InvalidateTransformedSelection();

            return prevMatrices;
        }
        public void ReaddSelectionTransform(Vex.Matrix prevTransforMatrix, Vex.Matrix[] prevMatrices)
        {
            uint[] selIds = Selection.SelectedIds;
            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.RemoveInstance(instanceManager[selIds[i]]);
            }

            for (int i = 0; i < selIds.Length; i++)
            {
                DesignInstance di = MainForm.CurrentInstanceManager[selIds[i]];
                di.SetMatrix(prevMatrices[i]);
            }
            MainForm.CurrentStage.Selection.TransformMatrix = prevTransforMatrix.SysMatrix();
            Selection.Update();
            
            for (int i = 0; i < selIds.Length; i++)
            {
                CurrentEditItem.SnapStore.AddInstance(instanceManager[selIds[i]]);
            }

            ResetTransformHandles();
            InvalidateTransformedSelection();
        }
        public void SetDesignInstanceMatrix(DesignInstance di, Vex.Matrix m)
        {
            CurrentEditItem.SnapStore.RemoveInstance(di);
            di.SetMatrix(m);
            CurrentEditItem.SnapStore.AddInstance(di);
        }

        public void AddGuide(Guide guide)
        {
            designStage.Guidelines.AddGuide(guide);
            designStage.SnapStore.AddInstance(guide);
            pb.Invalidate();
        }
        public void MoveGuide(Guide guide, int offsetX, int offsetY, List<Bond> addedBonds, List<Bond> previousBonds)
        {
            designStage.SnapStore.RemoveInstance(guide);
            guide.Move(offsetX, offsetY);
            designStage.SnapStore.AddInstance(guide);

            Bond guideBond = CurrentEditItem.BondStore.GetGuideBond(guide.InstanceHash);
            guideBond.GuideMoved = true;
            List<uint> guideIds = new List<uint>();
            CurrentEditItem.BondStore.GetBondsForGuide(guide.InstanceHash, guideIds);
            Selection.Set(guideIds.ToArray());
            TranslateSelection(new Vex.Point(offsetX, offsetY), false, addedBonds, previousBonds);
            guideBond.GuideMoved = false;

            pb.Invalidate();
        }
        public void RemoveGuide(Guide guide)
        {
            designStage.Guidelines.RemoveGuide(guide);
            designStage.SnapStore.RemoveInstance(guide);
            pb.Invalidate();
        }

        #endregion

        public void OnEdit()
        {
            designStage.LibraryItem.CalculateBounds();
        }
        public void SelectionChanged(object obj, EventArgs e)
        {
            OnSelectionChanged(this, Selection.Count);
            MainForm.PropertyBar.PopulateData(MainForm.CurrentStage.Selection);
        }
        public void UndoStackChanged(object obj, EventArgs e)
        {
            OnUndoStackChanged(this, EventArgs.Empty);
        }

        private void SetCursor(uint maskColor)
        {
            Cursor targetCursor = Cursor;
            ColorMask cm = (ColorMask)maskColor;

            if(keysDown.Contains(Keys.Space))
            {
                if (isMouseDown)
                {
                    targetCursor = CustomCursors.HandClosed;
                }
                else
                {
                    targetCursor = CustomCursors.HandPan;
                }
            }
            else if (dragKind == DragKind.Object)
            {
                if ((ModifierKeys & Keys.Control) != Keys.Control)
                {
                    targetCursor = CustomCursors.ArrowMove;
                }
                else
                {
                    targetCursor = CustomCursors.ArrowDup;
                }
            }
            else if (cm == 0)
            {
                targetCursor = CustomCursors.RectSelect;
            }
            else if (cm.IsObject())
            {
                targetCursor = CustomCursors.ArrowMove;
            }
            else if (maskColor == 0xFFFFFFFF)
            {
                targetCursor = CustomCursors.RectSelect;
            }
            else
            {
                if ((ModifierKeys & Keys.Control) != 0 && cm.IsHandle() && Selection.Count == 1)
                {
                    targetCursor = CustomCursors.Cross;
                }
                else
                {
                    switch (cm)
                    {
                        case ColorMask.CenterPoint:
                            targetCursor = CustomCursors.ArrowMoveCenter;
                            break;

                        case ColorMask.RotateBottomLeft:
                        case ColorMask.RotateBottomRight:
                        case ColorMask.RotateTopLeft:
                        case ColorMask.RotateTopRight:
                        case ColorMask.RotateLeftCenter:
                        case ColorMask.RotateRightCenter:
                        case ColorMask.RotateTopCenter:
                        case ColorMask.RotateBottomCenter:
                            targetCursor = CustomCursors.Rotate;
                            break;

                        case ColorMask.ScaleTopLeft:
                        case ColorMask.ScaleBottomRight:
                            targetCursor = CustomCursors.StretchNW_SE;
                            break;

                        case ColorMask.ScaleTopRight:
                        case ColorMask.ScaleBottomLeft:
                            targetCursor = CustomCursors.StretchNE_SW;
                            break;

                        case ColorMask.ScaleTopCenter:
                        case ColorMask.ScaleBottomCenter:
                            targetCursor = CustomCursors.StretchNS;
                            break;

                        case ColorMask.ScaleLeftCenter:
                        case ColorMask.ScaleRightCenter:
                            targetCursor = CustomCursors.StretchEW;
                            break;
                    }
                }
            }

            if(targetCursor != Cursor)
            {
                Cursor = targetCursor;
            }
        }

        #region Keyboard Events

        private HashSet<Keys> keysDown = new HashSet<Keys>();

        public void StageView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
            Vex.Rectangle prevSelBounds = Selection.StrokeBounds;

            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    // now handled in menu shortcuts
                    //if ((ModifierKeys & Keys.Control) == Keys.Control) // ctrl up or ctrlShift up is depth change
                    //{
                    //    AdjustSelectionDepthByKey(e.KeyCode);
                    //}
                    //else 

                    // nudging
                    if ((ModifierKeys & Keys.Control) != Keys.Control)
                    {
                        Nudge(e.KeyCode);
                    }
                    break;

                case Keys.Left:
                case Keys.Right:
                    Nudge(e.KeyCode);
                    break;

                case Keys.Delete:
                    MainForm.Instance.SelectionDelete(this, EventArgs.Empty);
                    break;

                case Keys.Escape:
                    MainForm.Instance.DocumentSelectNone(this, EventArgs.Empty);
                    break;

                case Keys.Space:
                    SetCursor(0);
                    break;

                case Keys.D:
                    if (!keysDown.Contains(Keys.D))
                    {
                        pb.Invalidate();
                    }
                    break;
                    
                case Keys.N:
                    if (!keysDown.Contains(Keys.N))
                    {
                        ToggleNameMode();
                    }
                    break;

                case Keys.R:
                    // ctrl R is import
                    if ((ModifierKeys & Keys.Control) != Keys.Control)
                    {
                        Repeat();
                    }
                    break;

                case Keys.D1:
                    if (!keysDown.Contains(Keys.D1))
                    {
                        ResetZoom();
                    }
                    break;

                case Keys.D2:
                    if (!keysDown.Contains(Keys.D2))
                    {
                        ZoomToFit();
                    }
                    break;

                case Keys.D3:
                    if (!keysDown.Contains(Keys.D3))
                    {
                        if (Selection.Count > 0)
                        {
                            ZoomToSelection();
                        }
                        else
                        {
                            ZoomToFit();
                        }
                    }
                    break;

                case Keys.OemMinus:
                    if (!keysDown.Contains(Keys.OemMinus))
                    {
                        ZoomSelection(-4);
                    }
                    break;

                case Keys.Oemplus:
                    if (!keysDown.Contains(Keys.Oemplus))
                    {
                        ZoomSelection(4);
                    }
                    break;

                case Keys.NumPad0:
                    if (!keysDown.Contains(Keys.NumPad0))
                    {
                        RemoveBondsFromSelection();
                    }
                    break;
                case Keys.NumPad1:
                    if (!keysDown.Contains(Keys.NumPad1) && MainForm.Instance.alignVerticalCentersToolStripMenuItem.Enabled)
                    {
                        AlignVerticalCenter();            
                    }
                    break;
                case Keys.NumPad2:
                    if (!keysDown.Contains(Keys.NumPad2) && MainForm.Instance.alignToBottomToolStripMenuItem.Enabled)
                    {
                        AlignToBottom();
                    }
                    break;
                case Keys.NumPad3:
                    if (!keysDown.Contains(Keys.NumPad3))
                    {
                    }
                    break;
                case Keys.NumPad4:
                    if (!keysDown.Contains(Keys.NumPad4) && MainForm.Instance.alignToLeftToolStripMenuItem.Enabled)
                    {
                        AlignToLeft();
                    }
                    break;
                case Keys.NumPad5:
                    if (!keysDown.Contains(Keys.NumPad5) && MainForm.Instance.distributeToLastAlignToolStripMenuItem.Enabled)
                    {
                        DistributeToLastAlign();
                    }
                    break;
                case Keys.NumPad6:
                    if (!keysDown.Contains(Keys.NumPad6) && MainForm.Instance.alignToRightToolStripMenuItem.Enabled)
                    {
                        AlignToRight();
                    }
                    break;
                case Keys.NumPad7:
                    if (!keysDown.Contains(Keys.NumPad7))
                    {
                    }
                    break;
                case Keys.NumPad8:
                    if (!keysDown.Contains(Keys.NumPad8) && MainForm.Instance.alignToTopToolStripMenuItem.Enabled)
                    {
                        AlignToTop();
                    }
                    break;
                case Keys.NumPad9:
                    if (!keysDown.Contains(Keys.NumPad9) && MainForm.Instance.alignHorizontalCentersToolStripMenuItem.Enabled)
                    {
                        AlignHorizontalCenter();
                    }
                    break;
            }
            keysDown.Add(e.KeyCode);

            if (ModifierKeys != prevModifierKeys)
            {
                prevModifierKeys = ModifierKeys;
                SetCursor(0);
            }
        }
        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            keysDown.Remove(e.KeyCode);
            switch (e.KeyCode)
            {
                case Keys.Space:
                    SetCursor(0);
                    break;

                case Keys.D:
                    pb.Invalidate();
                    break;

                case Keys.D1:
                    pb.Invalidate();
                    break;
            }

            if (ModifierKeys != prevModifierKeys)
            {
                prevModifierKeys = ModifierKeys;
                SetCursor(0);
            }
        }
        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'z':
                    MainForm.Instance.DocumentUndo(this, EventArgs.Empty);
                    break;
                case 'Z':
                case 'y':
                    MainForm.Instance.DocumentRedo(this, EventArgs.Empty);
                    break;
            }
        }
        #endregion

        #region Mouse Events

        // stage clicks
        float transformTranslateX;
        float transformTranslateY;
        float transformScaleX;
        float transformScaleY;
        float transformRotate;
        Vex.Point transformCenter;
        Vex.Point handleOffset;
        bool ctrlDownOnClick;

        DragKind dragKind;

        void StageView_MouseDown(object sender, MouseEventArgs e)
        {
            this.Parent.Select();
            isMouseDown = true;
            dragKind = DragKind.None;
            dragRect = Rectangle.Empty;
            mouseDownOriginCamera = new Point(e.X, e.Y);
            mouseDownOriginStage = Point.Round(CameraToStage(mouseDownOriginCamera));
            ctrlDownOnClick = (ModifierKeys & Keys.Control) != 0;

            if (clickMap != null)
            {
                clickedColor = clickMap.GetColorValueAt(mouseDownOriginCamera);
            }

            if(keysDown.Contains(Keys.Space) || e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                dragKind = DragKind.Pan;
                Cursor = CustomCursors.HandClosed;
                transformCenter = new Vex.Point(e.X, e.Y);
            }

            // test for aspect constrain line
            ColorMask cm = (ColorMask)clickedColor;
            if (Selection.Count == 1 && ctrlDownOnClick && (cm.IsScale() || cm.IsRotate())) // self constrain
            {
                int targIndex = cm.GetOppositeHandleIndex();
                transformHandles.AspectConstrainTarget = BondAttachmentExtensions.GetTargetFromHandleIndex(targIndex);
                clickMapNeedsUpdate = true;
                pb.Invalidate();
            }
        }             
        void BeginMove()
        {
            ColorMask cm = (ColorMask)clickedColor;

            if (cm > ColorMask.First && cm < ColorMask.Last)
            {
                if (cm == ColorMask.CenterPoint)
                {
                    dragKind = DragKind.CenterPoint;
                    transformCenter = Selection.GlobalRotationCenter;
                }
                else if (cm.IsScale() || cm.IsRotate())
                {
                    draggingElement = cm;
                    handleOffset = draggingElement.GetHandleOffset(Selection.StrokeBounds, mouseDownOriginStage.VexPoint());

                    if (Selection.Count == 1 && ctrlDownOnClick) // self constrain
                    {
                        dragKind = DragKind.AspectConstrain;
                    }
                    else
                    {
                        dragKind = cm.IsScale() ? DragKind.Scale : DragKind.Rotate;

                        if (dragKind == DragKind.Scale)
                        {
                            PointF[] pts = Selection.GetTransformedPoints();
                            transformCenter = draggingElement.GetScalingOrigin(pts).VexPoint();
                        }
                        else
                        {
                            transformCenter = Selection.GlobalRotationCenter;
                        }
                    }
                    InvalidateSelection();
                }
                else if (cm.IsRuler())
                {
                    switch (cm)
                    {
                        case ColorMask.RulerSelWidth:
                        case ColorMask.RulerSelHeight:
                            dragKind = DragKind.Object;
                            draggingElement = cm;
                            break;
                        case ColorMask.RulerTop:
                            dragKind = DragKind.GuideHorizontal;
                            draggingElement = cm;
                            break;
                        case ColorMask.RulerSide:
                            dragKind = DragKind.GuideVertical;
                            draggingElement = cm;
                            break;
                        case ColorMask.RulerCorner:
                            dragKind = DragKind.GuideRectangle;
                            draggingElement = cm;
                            break;
                    }
                }
            }
            else
            {
                // check for object drag
                uint id = GetIndexFromMouseOrigin();
                if (designStage.Contains(id))
                {
                    dragKind = DragKind.Object;
                    if (!designStage[id].IsSelected)
                    {
                        commandStack.Do(new SelectInstancesCommand(new uint[] { id }, SelectionModifier.SetSelection));
                        InvalidateSelection();
                    }

                    if (dragInCursor)
                    {
                        DoDragDrop(designStage.Selected, DragDropEffects.Move);
                    }
                }
                else if(designStage.Guidelines.GetGuideFromColor((uint)cm) != null)
                {
                    Guide guide = designStage.Guidelines.GetGuideFromColor((uint)cm);
                    dragKind = guide.GetDragKind();
                    draggingElement = cm;
                }
                else
                {
                    dragKind = DragKind.RectangleSelect;
                }
            }
        }
        void StageView_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                switch (dragKind)
                {
                    case DragKind.None:
                        if (TriggeredDrag(e.X, e.Y))
                        {
                            BeginMove();
                            mouseCurrentCamera = e.Location;
                        }
                        else
                        {
                            dragRect = mouseDownOriginCamera.GetPositiveRect(e.Location);
                        }
                        break;

                    case DragKind.Pan:
                        float ptX = (e.Location.X - transformCenter.X);
                        float ptY = (e.Location.Y - transformCenter.Y);
                        cameraMatrix.Translate(ptX, ptY, MatrixOrder.Append);
                        transformCenter = new Vex.Point(e.Location.X, e.Location.Y);
                        pb.Invalidate();
                        break;

                    case DragKind.RectangleSelect:
                        dragRect = mouseDownOriginCamera.GetPositiveRect(e.Location);
                        InvalidateBorder(dragRect);
                        break;

                    case DragKind.AspectConstrain:
                        mouseCurrentCamera = e.Location;
                        Rectangle invRect = dragRect;
                        dragRect = mouseDownOriginCamera.GetPositiveRect(e.Location);
                        dragRect.Inflate(new Size(5,5));
                        pb.Invalidate(Rectangle.Union(dragRect, invRect));
                        break;

                    case DragKind.GuideRectangle:
                        if (IsEditingRoot)
                        {
                            Point center = Point.Round(StageToCamera(vexObject.ViewPort.LocalCenter.SysPointF()));
                            Size sz = new Size((center.X - e.Location.X) * 2, (center.Y - e.Location.Y) * 2);
                            dragRect = new Rectangle(e.Location, sz);
                        }
                        else
                        {
                            Point org = Point.Round(StageToCamera(PointF.Empty));
                            dragRect = org.GetPositiveRect(e.Location);
                        }
                        InvalidateBorder(dragRect);
                        break;

                    case DragKind.GuideHorizontal:
                        dragRect = new Rectangle(new Point(0, (int)e.Location.Y - 1), new Size(CanvasSize.Width, 2));
                        //Rectangle locRect = Rectangle.Round(CameraToStage(dragRect));
                        InvalidateBorder(dragRect);
                        break;
                    case DragKind.GuideVertical:
                        dragRect = new Rectangle(new Point((int)e.Location.X - 1, 0), new Size(2, CanvasSize.Height));
                        InvalidateBorder(dragRect);
                        break;

                    case DragKind.CenterPoint:
                        Selection.GlobalRotationCenter = CameraToStage(e.Location).VexPoint();
                        centerNeedsInvalidate = true;
                        InvalidateSelection();
                        break;

                    case DragKind.Object:
                        PointF mouseCurrent = CameraToStage(e.Location);
                        Vex.Point constrainedOffset = new Vex.Point(mouseCurrent.X - mouseDownOriginStage.X, mouseCurrent.Y - mouseDownOriginStage.Y);
                        CurrentEditItem.BondStore.ConstrainOffset(Selection.SelectedIds, ref constrainedOffset);
                        transformTranslateX = constrainedOffset.X;
                        transformTranslateY = constrainedOffset.Y;

                        PointF[] pts = (Selection.IsSingleRotated) ? Selection.GetTransformedCenter() : Selection.GetTransformedPoints().GetMidpointsAndCenter();
                        //PointF[] pts = Selection.GetTransformedPoints().GVeetMidpointsAndCenter();
                        pts.TranslatePoints(transformTranslateX, transformTranslateY);

                        designStage.SnapStore.SnapDistance = 
                            (int)(designStage.SnapStore.unzoomedSnapDistance * 
                            1f / Math.Max(cameraMatrix.Elements[0], cameraMatrix.Elements[3]));

                        for (int i = 0; i < snapPoints.Length; i++)
			            {
                            snapPoints[i] = Vex.Point.Empty;
                            snapTypes[i] = BondAttachment.None;
			            }

                        if (SnapToObjects || ShowGuides)
                        {
                            Vex.Point offset = designStage.SnapStore.GetSnapPoints(pts, ref snapPoints, ref snapTypes);
                            transformTranslateX += offset.X;
                            transformTranslateY += offset.Y;
                        }

                        if (draggingElement == ColorMask.RulerSelWidth)
                        {
                            transformTranslateY = 0;
                        }
                        else if (draggingElement == ColorMask.RulerSelHeight)
                        {
                            transformTranslateX = 0;
                        }
                        else if ((ModifierKeys & Keys.Shift) != 0)
                        {
                            if (Math.Abs(transformTranslateX) > Math.Abs(transformTranslateY))
                            {
                                transformTranslateY = 0;
                            }
                            else
                            {
                                transformTranslateX = 0;
                            }
                        }
                        

                        InvalidateTransform();
                        SetCursor(0);
                        break;

                    case DragKind.Scale:
                        Vex.Point dragCurrent = CameraToStage(e.Location).VexPoint();

                        if (draggingElement.IsRuler())
                        {
                            Vex.Rectangle sb = Selection.StrokeBounds;
                            transformScaleX =  draggingElement.IsLeft() ?
                                (sb.Right - dragCurrent.X) / sb.Width:
                                (dragCurrent.X - sb.Left) / sb.Width;
                            transformScaleY = draggingElement.IsTop() ?
                                (sb.Bottom - dragCurrent.Y) / sb.Height :
                                (dragCurrent.Y - sb.Top) / sb.Height;
                        }
                        else
                        {
                            Vex.Rectangle sb = Selection.UntransformedBounds;
                            Vex.Point localDrag = Selection.TransformToLocal(dragCurrent);
                            Vex.Point locCenter = Selection.TransformToLocal(transformCenter);

                            transformScaleX = draggingElement.IsLeft() ?
                                (locCenter.X - localDrag.X) / sb.Width :
                                (localDrag.X - locCenter.X) / sb.Width;

                            transformScaleY = draggingElement.IsTop() ?
                                (locCenter.Y - localDrag.Y) / sb.Height :
                                (localDrag.Y - locCenter.Y) / sb.Height;
                        }

                        if (draggingElement == ColorMask.ScaleLeftCenter || draggingElement == ColorMask.ScaleRightCenter ||
                            draggingElement == ColorMask.RulerSelLeft || draggingElement == ColorMask.RulerSelRight)
                        {
                            transformScaleY = 1;
                        }
                        else if (draggingElement == ColorMask.ScaleTopCenter || draggingElement == ColorMask.ScaleBottomCenter ||
                            draggingElement == ColorMask.RulerSelTop || draggingElement == ColorMask.RulerSelBottom)
                        {
                            transformScaleX = 1;
                        }
                        else if (!draggingElement.IsScaleConstrained() && (ModifierKeys & Keys.Shift) == 0)
                        {
                            float min = Math.Min(transformScaleX, transformScaleY);
                            transformScaleX = min;
                            transformScaleY = min;
                        }

                        SetCursor(0);
                        InvalidateTransform();
                        break;

                    case DragKind.Rotate:
                        float orgAngle = (float)Math.Atan2(mouseDownOriginStage.Y - transformCenter.Y, mouseDownOriginStage.X - transformCenter.X);
                        PointF curStagePoint = CameraToStage(e.Location);
                        float angle = (float)Math.Atan2(curStagePoint.Y - transformCenter.Y, curStagePoint.X - transformCenter.X);
                        transformRotate = (float)((angle - orgAngle) * 180f / Math.PI);

                        if ((ModifierKeys & Keys.Shift) != 0)
                        {
                            transformRotate = (float)Math.Round(transformRotate / 15f) * 15f;
                        }

                        SetCursor(0);
                        InvalidateTransform();
                        break;
                }
            }
            else if(clickMap != null)
            {
                uint id = clickMap.GetColorValueAt(new Point(e.X, e.Y));
                SetCursor(id);
            }
        }
        void StageView_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            if (!cancelMouseUp)
            {
                uint id = GetIndexFromMouseOrigin();
                uint[] selectedIds = new uint[] { };
                bool isRectSelect = false;
                Rectangle rectSelectBounds = Rectangle.Empty;

                if (dragKind == DragKind.RectangleSelect) // making selection rect
                {
                    isRectSelect = true;
                    rectSelectBounds = mouseDownOriginCamera.GetPositiveRect(e.Location);
                    rectSelectBounds.Intersect(Rectangle.Truncate(prevVisibleClipBounds));
                    selectedIds = clickMap.GetIdsInRect(rectSelectBounds);
                }
                else if (dragKind.IsGuide())
                {
                    Rectangle locRect = Rectangle.Round(CameraToStage(dragRect));
                    bool isValid = (dragRect.Width >= 0 || dragRect.Height >= 0) && pb.Bounds.Contains(e.Location);

                    // moving guide
                    if (designStage.Guidelines.GetGuideFromColor((uint)draggingElement) != null)
                    {
                        Guide guide = designStage.Guidelines.GetGuideFromColor((uint)draggingElement);
                        if (isValid)
                        {                            int offsetX = (guide.GuideType == GuideType.Horizontal) ? 0 : (int)(locRect.X - guide.Bounds.Left);
                            int offsetY = (guide.GuideType == GuideType.Vertical) ? 0 : (int)(locRect.Y - guide.Bounds.Top);
                            commandStack.Do(new MoveGuideCommand(guide, offsetX, offsetY));
                        }
                        else
                        {
                            commandStack.Do(new RemoveGuideCommand(guide));
                        }
                    }
                    else // new guide
                    {
                        if (isValid)
                        {
                            Rectangle clampRect = locRect;
                            if (IsEditingRoot)
                            {
                                clampRect.Intersect(vexObject.ViewPort.SysRectangle());
                            }

                            Guide g;
                            switch (dragKind)
                            {
                                case DragKind.GuideHorizontal:
                                    g = new Guide(clampRect.Left, locRect.Top, clampRect.Right, locRect.Top);
                                    commandStack.Do(new AddGuideCommand(g));
                                    break;
                                case DragKind.GuideVertical:
                                    g = new Guide(locRect.Left, clampRect.Top, locRect.Left, clampRect.Bottom);
                                    commandStack.Do(new AddGuideCommand(g));
                                    break;
                                case DragKind.GuideRectangle:
                                    if (locRect.X >= 0 && locRect.Y >= 0)
                                    {
                                        g = new Guide(locRect.VexRectangle());
                                        commandStack.Do(new AddGuideCommand(g));
                                    }
                                    break;
                            }
                        }
                    }
                    pb.Invalidate();
                }
                else if (dragKind == DragKind.Object && !dragInCursor)
                {
                    if ((ModifierKeys & Keys.Control) == 0)
                    {
                        commandStack.Do(new MoveSelectionCommand(new Vex.Point(transformTranslateX, transformTranslateY)));
                    }
                    else
                    {
                        // duplicate
                        Vex.Point offset = new Vex.Point(transformTranslateX, transformTranslateY);
                        CommandStack.Do(new DuplicateSelectedCommand(offset));
                    }
                    draggingElement = ColorMask.Last;
                }
                else if (dragKind == DragKind.Scale)
                {
                    if ((ModifierKeys & Keys.Control) == 0)
                    {
                        commandStack.Do(new ScaleTransformCommand(transformScaleX, transformScaleY, transformCenter));
                    }
                    else
                    {
                        commandStack.Do(
                            new GroupCommand(
                                new DuplicateSelectedCommand(Vex.Point.Zero), 
                                new ScaleTransformCommand(transformScaleX, transformScaleY, transformCenter)));
                    }
                    draggingElement = ColorMask.Last;
                }
                else if (dragKind == DragKind.Rotate)
                {
                    if ((ModifierKeys & Keys.Control) == 0)
                    {
                        Rotate(transformRotate);
                    }
                    else
                    {
                        commandStack.Do(
                            new GroupCommand(
                                new DuplicateSelectedCommand(Vex.Point.Zero),
                                new RotateTransformCommand(transformRotate)));
                    }
                    draggingElement = ColorMask.Last;
                }
                else if (dragKind == DragKind.CenterPoint)
                {
                    commandStack.Do(new TranslateRotationCenterCommand(transformCenter, CameraToStage(e.Location).VexPoint()));
                }
                else if (dragKind == DragKind.AspectConstrain)
                {
                    DesignInstance di = instanceManager[Selection.SelectedIds[0]];
                    if(di.Instance is Vex.Instance)
                    {
                        Vex.AspectConstraint ac = ((Vex.Instance)di.Instance).AspectConstraint;
                        Vex.AspectConstraint newAc;
                        ColorMask cm = (ColorMask)clickMap.GetColorValueAt(mouseCurrentCamera);
                        if(cm.IsScale() || cm.IsRotate()) // there are only valid targets in AspectContraint drag mode
                        {
                            newAc = transformHandles.AspectConstrainTarget.GetAspectConstraint();
                        }
                        else
                        {
                            newAc = Vex.AspectConstraint.None;
                        }

                        if (ac != newAc)
                        {
                            commandStack.Do(new ConstrainAspectCommand(newAc));
                        }
                        else
                        {
                            pb.Invalidate();
                        }
                    }
                }
                else if (id <= 0xFFFF) // click select
                {
                    selectedIds = new uint[] { id };
                }

                if (selectedIds.Length > 0)
                {
                    if ((ModifierKeys & Keys.Shift) == 0)
                    {
                        // not holding down shift
                        if (!isRectSelect)
                        {
                            if (instanceManager[id].IsSelected)
                            {
                                // clicked selection, adjust transform handles
                                transformHandles.ToggleKind();
                                selectionNeedsInvalidate = true;
                            }
                            else
                            {
                                commandStack.Do(new SelectInstancesCommand(selectedIds, SelectionModifier.SetSelection));
                            }
                        }
                        else
                        {
                            if (!Selection.IsMatchingSet(selectedIds))
                            {
                                commandStack.Do(new SelectInstancesCommand(selectedIds, SelectionModifier.SetSelection));
                            }
                        }
                    }
                    else
                    {
                        // holding down shift
                        if (isRectSelect)
                        {
                            uint[] newSelection = selectedIds.Union(Selection.SelectedIds).ToArray<uint>();
                            if (!Selection.IsMatchingSet(newSelection))
                            {
                                commandStack.Do(new SelectInstancesCommand(selectedIds, SelectionModifier.AddToSelection));
                            }
                        }
                        else if (designStage[id].IsSelected)
                        {
                            commandStack.Do(new SelectInstancesCommand(selectedIds, SelectionModifier.SubtractFromSelection));
                        }
                        else
                        {
                            commandStack.Do(new SelectInstancesCommand(selectedIds, SelectionModifier.AddToSelection));
                        }
                    }

                    if (isRectSelect)
                    {
                        InvalidateBorder(rectSelectBounds);
                    }

                    this.InvalidateSelection();
                }
                else if (!Selection.IsEmpty &&
                         (dragKind == DragKind.None || dragKind == DragKind.RectangleSelect)) // rect deselects all
                {
                    commandStack.Do(new SelectInstancesCommand(selectedIds, SelectionModifier.SetSelection));
                    this.InvalidateSelection();
                }
            }

            cancelMouseUp = false;
            StopDrag();
            dragKind = DragKind.None;
        }
        void StageView_MouseWheel(object sender, MouseEventArgs e)
        {
            Zoom(e.Delta, e.Location);
        }
        void pb_DoubleClick(object sender, EventArgs e)
        {
            uint id = GetIndexFromMouseOrigin();
            if(id >= 0x00FFFF00) // clicked whitespace, pop out
            {
                if(editStack.Count > 1)
                {
                    commandStack.Do(new EditInPlaceCommand(1));
                    cancelMouseUp = true;
                }
            }
            else // clicked object, push in
            {
                commandStack.Do(new EditInPlaceCommand(id));
                cancelMouseUp = true;
            }
        }

        public void EditInPlacePush(uint id)
        {
            DesignInstance di = instanceManager[id];
            if (di is DesignTimeline)
            {
                if (designStage != null)
                {
                    designStage.Selected.ContentsChanged -= SelectionChanged;
                }
                cameraMatrix.Multiply(di.GetSysMatrix(), MatrixOrder.Prepend);
                editStack.Push((DesignTimeline)di);

                designStage = editStack.Peek();
                designStage.Selected.ContentsChanged += SelectionChanged;
                Selection.Set(new uint[] { });
                designStage.EnsureSnaps();
                pb.Invalidate();

                OnNestedEditChanged(this, EventArgs.Empty);
            }
        }

        public uint[] EditInPlacePop(int popCount)
        {
            uint[] result = new uint[popCount];

            for (int i = 0; i < popCount; i++)
            {
                result[i] = EditInPlacePop();
            }

            Selection.Set(new uint[] { });
            pb.Invalidate();
            OnNestedEditChanged(this, EventArgs.Empty);

            return result;
        }
        private uint EditInPlacePop()
        {
            uint result = 0;
            if (editStack.Count > 1)
            {
                if (designStage != null)
                {
                    designStage.Selected.ContentsChanged -= SelectionChanged;
                }

                using (Matrix m = designStage.GetSysMatrix().Clone())
                {
                    m.Invert();
                    cameraMatrix.Multiply(m, MatrixOrder.Prepend);
                }
                result = editStack.Pop().InstanceHash;

                designStage = editStack.Peek();
                designStage.Selected.ContentsChanged += SelectionChanged;
                designStage.EnsureSnaps();
            }
            return result;
        }


        private bool TriggeredDrag(int movedX, int movedY)
        {
            return Math.Abs(movedX - mouseDownOriginCamera.X) > SystemInformation.DragSize.Width ||
                   Math.Abs(movedY - mouseDownOriginCamera.Y) > SystemInformation.DragSize.Height;
        }

        private void AdjustSelectionDepthByKey(Keys key)
        {
            bool toEnd = (ModifierKeys & Keys.Shift) == Keys.Shift;
            int dir = (key & Keys.Up) == Keys.Up ? 1 : -1;
            AdjustSelectionDepth(dir, toEnd, true);
        }

        public bool AdjustSelectionDepth(int dir, bool toEnd, bool apply)
        {
            List<int> oldDepths = new List<int>();
            List<int> newDepths = new List<int>();

            int top = designStage.Count;
            int bottom = -1;
            uint[] selectedIds = Selection.IdsByDepth;
            if (dir > 0)
            {
                for (int i = selectedIds.Length - 1; i >= 0; i--)
                {
                    int depth = designStage.GetDepth(selectedIds[i]);
                    if (depth + dir < top)
                    {
                        oldDepths.Add(depth);
                        int targ = top - 1; // toEnd
                        if (!toEnd)
                        {
                            targ = depth + dir;
                            top++;
                        }
                        newDepths.Add(targ);
                    }
                    top--;
                }
            }
            else
            {
                for (int i = 0; i < selectedIds.Length; i++)
                {
                    int depth = designStage.GetDepth(selectedIds[i]);
                    if (depth + dir > bottom)
                    {
                        oldDepths.Add(depth);
                        int targ = bottom + 1; // toEnd
                        if (!toEnd)
                        {
                            targ = depth + dir;
                            bottom--;
                        }
                        newDepths.Add(targ);
                    }
                    bottom++;
                }
            }

            bool hasChange = oldDepths.Count > 0;
            if (apply)
            {
                if (hasChange)
                {
                    commandStack.Do(new SwapDepthsCommand(oldDepths.ToArray(), newDepths.ToArray()));
                    OnDepthChanged(this, EventArgs.Empty);
                }

                selectionNeedsInvalidate = true;
                InvalidateSelection();
            }

            return hasChange;
        }
        public bool IsTop()
        {
            return !AdjustSelectionDepth(1, true, false);
        }
        public bool IsBottom()
        {
            return !AdjustSelectionDepth(-1, true, false);
        }

        private void Nudge(Keys key)
        {
            int amount = (ModifierKeys & Keys.Shift) == Keys.Shift ? superNudge : nudge;
            int amountX = 0;
            int amountY = 0;

            switch (key)
            {
                case Keys.Up:
                    amountY = -amount;
                    break;
                case Keys.Down:
                    amountY = amount;
                    break;
                case Keys.Left:
                    amountX = -amount;
                    break;
                case Keys.Right:
                    amountX = amount;
                    break;
            }

            if (commandStack.Peek() is NudgeSelectionCommand)
            {
                NudgeSelectionCommand nsc = (NudgeSelectionCommand)commandStack.Peek();
                nsc.AppendNudge(amountX, amountY);
            }
            else
            {
                commandStack.Do(new NudgeSelectionCommand(amountX, amountY));
            }
            InvalidateSelection();
        }
        private uint GetIndexFromMouseOrigin()
        {
            uint result = uint.MaxValue;
            if (clickMap != null)
            {
                result = clickMap.GetColorValueAt(mouseDownOriginCamera) & 0x00FFFFFF; // not using alpha for debugging
            }
            return result;
        }

        #endregion

        #region Drag Drop

        private void StopDrag()
        {
            InvalidateBorder(dragRect);

            dragKind = DragKind.None;
            dragRect = Rectangle.Empty;
           //transformRect = Rectangle.Empty;

            prevTransformRect = Rectangle.Empty;
            mouseDownOriginCamera = Point.Empty;
            clickedColor = 0xFFFFFFFF;
            //snapPoints = new Vex.Point[]{};

            transformTranslateX = 0;
            transformTranslateY = 0;
            transformScaleX = 1;
            transformScaleY = 1;
            transformRotate = 0;

            transformHandles.AspectConstrainTarget = BondAttachment.None;

            if (screenCaptureCache != null)
            {
                screenCaptureCache.Dispose();
                screenCaptureCache = null;
            }
            InvalidateSelection();
        }

        void StageView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = e.Effect != DragDropEffects.Move;
        }

        void StageView_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
			IDataObject dobj = e.Data;
            if (dragInCursor && e.Data.GetDataPresent(InstanceGroup.formatName))
            {
                e.Effect = DragDropEffects.Move;
                InstanceGroup ig = (InstanceGroup)e.Data.GetData(InstanceGroup.formatName);

                PointF bp = StageToCamera(ig.StrokeBounds.Point.SysPointF());
                Vex.Point localCenter = new Vex.Point(
                    (int)(mouseDownOriginCamera.X - bp.X),
                    (int)(mouseDownOriginCamera.Y - bp.Y));

                Bitmap img = ig.GetOutline();
                AddDragImageToCursor(img, localCenter.SysPoint());
                img.Dispose(); // non raw elements are never cached
			}
			else if (e.Data.GetDataPresent(LibraryItemDragPacket.formatName))
			{
				e.Effect = DragDropEffects.Link;
				LibraryItemDragPacket lidp = (LibraryItemDragPacket)e.Data.GetData(LibraryItemDragPacket.formatName);
				if (lidp.Items != null && lidp.Items.Length > 0) 
				{
					LibraryItem li = lidp.Items [0];
					Vex.Size sz = li.Size;

					Bitmap img = li.GetScaledImage (cameraMatrix);
					Point center = new Point ((int)(img.Width / 2.0), (int)(img.Height / 2.0));
					AddDragImageToCursor (img, center);
				}
			}
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                bool hif = Library.HasImportableFile((string[])e.Data.GetData(DataFormats.FileDrop));
                if( hif )
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        void StageView_DragOver(object sender, DragEventArgs e)
        {
            if (dragInCursor && e.Data.GetDataPresent(InstanceGroup.formatName))
            {
                Point p = new Point(e.X, e.Y);
                Point pc = pb.PointToClient(p);
                PointF mouseCurrent = CameraToStage(pc);
                transformTranslateX = mouseCurrent.X - mouseDownOriginStage.X;
                transformTranslateY = mouseCurrent.Y - mouseDownOriginStage.Y;
                InvalidateTransform();
            }
        }
        void StageView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(InstanceGroup.formatName))
            {
                // dragging for a move
                Point dropLoc = ((Control)sender).PointToClient(new Point(e.X, e.Y));
                PointF tDropLoc = CameraToStage(dropLoc);
                Vex.Point diff = new Vex.Point(tDropLoc.X - mouseDownOriginStage.X, tDropLoc.Y - mouseDownOriginStage.Y);

                commandStack.Do(new MoveSelectionCommand(diff));

                InvalidateSelectionOffset(diff.Negate().SysPoint());
                InvalidateSelection();
			}
			else if (e.Data.GetDataPresent(LibraryItemDragPacket.formatName))
			{
				e.Effect = DragDropEffects.Link;
				LibraryItemDragPacket lidp = (LibraryItemDragPacket)e.Data.GetData(LibraryItemDragPacket.formatName);
				if (lidp.Items != null && lidp.Items.Length > 0) 
				{
					// new element from library
					LibraryItem li = lidp.Items[0];
					if (designStage.CanAdd (li)) {
						Point dropLoc = ((Control)sender).PointToClient (new Point (e.X, e.Y));
						PointF tDropLoc = CameraToStage (dropLoc);

						// center it
						Vex.Rectangle sb = li.Definition.StrokeBounds;
						Vex.Point final = new Vex.Point (tDropLoc.X - sb.Width / 2 - sb.Left, tDropLoc.Y - sb.Height / 2 - sb.Top);

						CommandStack.Do (new AddInstancesCommand (new uint[] { li.DefinitionId }, new Vex.Point[] { final }));
						wasTransformed = true;
						InvalidateSelection ();
					} else {
						StopDrag ();
						MessageBox.Show ("        Can not add a symbol to itself.", "UI Layout Tool");
					}
				}
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                this.Focus();
                Point dropLoc = ((Control)sender).PointToClient(new Point(e.X, e.Y));
                Vex.Point loc = CameraToStage(dropLoc).VexPoint();

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Library.HasImportableFile(files))
                {
                    foreach (string file in files)
                    {
                        string ext = Path.GetExtension(file).ToUpperInvariant();
                        if (Library.ImportableFileExtensions.IndexOf(ext) > -1)
                        {
                            CommandStack.Do(new ImportFileCommand(file, loc));
                        }
                    }
                }
            }

            this.Parent.Select();

            StopDrag();
        }
        
        void AddDragImageToCursor(Bitmap bmp, Point center)
        {
            Size sz = bmp.Size;
            Size isz = sz;
            Size csz = Properties.Resources.addSymbolCursor.Size;
            if (isz.Width < csz.Width * 2)
            {
                isz = new Size(isz.Width / 2 + csz.Width, isz.Height);
            }
            if (isz.Height < csz.Height * 2)
            {
                isz = new Size(isz.Width, isz.Height / 2 + csz.Height);
            }

            FastBitmap fbmp = new FastBitmap(new Bitmap(isz.Width, isz.Height));
            Graphics g = Graphics.FromImage(fbmp.Bitmap);
            g.DrawImageUnscaled(bmp, Point.Empty);
            fbmp.MakeTransparent(.85f);

            g.DrawImageUnscaled(Properties.Resources.addSymbolCursor, center);

            Cursor.Current = CreateCursor(fbmp.Bitmap, center);
            fbmp.Dispose();
        }
        #endregion

        #region Paint

        List<Vex.Point> invalidCenters = new List<Vex.Point>();
        List<Rectangle> invalidAreas = new List<Rectangle>();

        public void InvalidateAll()
        {
            pb.Invalidate();
        }
        public void InvalidateTransformedSelection()
        {
            // todo: look only for identical timelines in background if not editing root
            wasTransformed = true;
            InvalidateSelection();
        }
        public void InvalidateCenterPoint()
        {
            transformHandles.SetAsRotate();
            centerNeedsInvalidate = true;
            selectionNeedsInvalidate = true;
            InvalidateSelection();
            prevCenterPoint = Point.Empty;
        }
        public void InvalidateSelection()
        {
            PointF[] pts = TransformPointsCamera;
            Rectangle r = TransformHandles.GetInvalidBounds(pts);

            InvalidateRulers();

            if (wasTransformed || Selection.RelatedAreInvalid) 
            {
                // changed current edit level, 
                // there may be a parent symbol that needs a redraw
                // also need to check library image
                LibraryView.Instance.ResetImage();
                pb.Invalidate();
                wasTransformed = false;
                Selection.RelatedAreInvalid = false;
            }
            else if (selectionNeedsInvalidate || !r.Equals(prevInvalidateRect))
            {
                pb.Invalidate(r);
                if (!r.Contains(prevInvalidateRect))
                {
                    pb.Invalidate(prevInvalidateRect);
                }
                prevInvalidateRect = r;


                if (isDebugMode)
                {
                    invalidAreas.Add(r);
                }
            }

            PointF cp = RotationCenterToCamera;
            if (centerNeedsInvalidate || selectionNeedsInvalidate || cp != prevCenterPoint)
            {
                Rectangle ir;
                if (prevCenterPoint != Point.Empty)
                {
                    ir = TransformHandles.GetCenterBoundsFromPoint(prevCenterPoint);
                    pb.Invalidate(ir);
                }

                if (transformHandles.TransformKind == TransformKind.Rotate)
                {
                    ir = TransformHandles.GetCenterBoundsFromPoint(cp);
                    pb.Invalidate(ir);
                }
                prevCenterPoint = cp;
            }

            int selCount = Selection.Count;
            int prevCount = invalidCenters.Count;
            int centersCount = Math.Max(selCount, prevCount);
            int index = centersCount - 1;

            while (centersCount > 0 && index >= 0)
            {
                if(index < selCount && index < prevCount)
                {
                    Vex.Point p = instanceManager[Selection[index]].Location;
                    if (invalidCenters[index] != p)
                    {
                        pb.Invalidate(GetLocationBox(invalidCenters[index]));
                        pb.Invalidate(GetLocationBox(p));
                        invalidCenters[index] = p;
                    }
                }
                else if (index > Selection.Count)
                {
                    pb.Invalidate(GetLocationBox(invalidCenters[index]));
                    invalidCenters.RemoveAt(index);
                }
                else if (index >= invalidCenters.Count)
                {
                    Vex.Point p = instanceManager[Selection[index]].Location;
                    pb.Invalidate(GetLocationBox(p));
                    invalidCenters.Add(p);
                }

                index--;
            }

            selectionNeedsInvalidate = false;
            centerNeedsInvalidate = false;
        }
        private void InvalidateTransform()
        {
            pb.Invalidate(); // todo: revisit transform invalidation

            InvalidateRulers();

            PointF[] pts;
            //using (Matrix m = designStage.GetSysMatrix().Clone())
            using (Matrix m = GetDynamicMatrix())
            {
                m.Multiply(cameraMatrix, MatrixOrder.Append);
                pts = Selection.GetTransformedPoints(m);
            }

            Rectangle r = pts.GetBounds();
            pb.Invalidate(r);

            if (prevTransformRect != Rectangle.Empty)
            {
                pb.Invalidate(prevTransformRect);
            }

            if (isDebugMode)
            {
                invalidAreas.Add(r);
            }

            prevTransformRect = r;
        }
        public void InvalidateRulers()
        {
            if (IsEditingRoot)
            {
                pb.Invalidate(new Rectangle(0, 0, pb.Width, rulers.RulerSize));
                pb.Invalidate(new Rectangle(0, 0, rulers.RulerSize, pb.Height));
            }
            else
            {
                pb.Invalidate();
            }
        }

        private int locationRadius = 5;
        private Rectangle GetLocationBox(Vex.Point pt)
        {
            Rectangle r = new Rectangle(
                (int)Math.Floor(pt.X) - locationRadius - 1,
                (int)Math.Floor(pt.Y) - locationRadius - 1, 
                locationRadius * 2 + 3, locationRadius * 2 + 3);

            return Rectangle.Ceiling(StageToCamera(r));
        }

        public Matrix GetDynamicMatrix()
        {
            Matrix m = new Matrix();
            
            if (dragKind == DragKind.Object)
            {
                m.Translate(transformTranslateX, transformTranslateY, MatrixOrder.Append);
            }
            else if (dragKind == DragKind.Scale)
            {
                m.ScaleAt(transformScaleX, transformScaleY, transformCenter.SysPointF());
            }
            else if (dragKind == DragKind.Rotate)
            {
                m.RotateAt(transformRotate, transformCenter.SysPointF(), MatrixOrder.Append);
            }

            return m;
        }


        private void InvalidateSelectionOffset(Point offset)
        {
            Rectangle r = Selection.StrokeBounds.SystemRectangle;
            Rectangle prevLoc = new Rectangle(r.X + offset.X - 1, r.Y + offset.Y - 1, r.Width + 2, r.Height + 2);
            pb.Invalidate(prevLoc);

            if (isDebugMode)
            {
                invalidAreas.Add(r);
            }
        }
        private void InvalidateBorder(Rectangle r0)
        {
            if(prevDragRect == Rectangle.Empty)
            {
                prevDragRect = r0;
            }
            int extra = 2;
            int lout = Math.Min(r0.Left, prevDragRect.Left) - extra;
            int lin = Math.Max(r0.Left, prevDragRect.Left) + extra;

            int tout = Math.Min(r0.Top, prevDragRect.Top) - extra;
            int tin = Math.Max(r0.Top, prevDragRect.Top) + extra;

            int rout = Math.Max(r0.Right, prevDragRect.Right) + extra;
            int rin = Math.Min(r0.Right, prevDragRect.Right) - extra;

            int bout = Math.Max(r0.Bottom, prevDragRect.Bottom) + extra;
            int bin = Math.Min(r0.Bottom, prevDragRect.Bottom) - extra;

            pb.Invalidate(new Rectangle(lout, tout, lin - lout, bout - tout)); // left
            pb.Invalidate(new Rectangle(lout, tout, rout - lout, tin - tout)); // top
            pb.Invalidate(new Rectangle(rin, tout,  rout - rin, bout - tout)); // right
            pb.Invalidate(new Rectangle(lout, bin,  rout - lout, bout - bin)); // bottom

            prevDragRect = r0;
        }

        private void UpdateClickMap(PaintEventArgs e)
        {
            if (prevVisibleClipBounds != pb.Bounds)
            {
                if (clickMap != null)
                {
                    clickMap.Dispose();
                }
                prevVisibleClipBounds = pb.Bounds;
                clickMap = new FastBitmap(new Bitmap((int)prevVisibleClipBounds.Width, (int)prevVisibleClipBounds.Height));
            }

            lock (clickMap)
            {
                Graphics g = clickMap.Graphics;
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.FillRectangle(Brushes.White, g.VisibleClipBounds);

                GraphicsState gstate = g.Save();
                g.Transform = cameraMatrix;

                if (showGuides)
                {
                    designStage.Guidelines.DrawMask(g);
                }

                designStage.DrawMask(g);

                if (!Selection.IsEmpty)
                {
                    transformHandles.DrawMask(clickMap);
                }

                g.Restore(gstate);
                if (showRulers)
                {
                    rulers.DrawMask(g);
                }
            }
        }
        protected void OnPBPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.Transform = cameraMatrix;
            g.Clear(stageColor);

            uint[] selIds = Selection.SelectedIds;
            if (!IsEditingRoot)
            {
                using (Matrix rootM = GetCalculatedRootMatrix())
                {
                    // fade background
                    g.Transform = rootM;
                    g.FillRectangle(paperColor, vexObject.ViewPort.SysRectangle());
                    root.Draw(g);
                    g.Transform = identityMatrix;
                    SolidBrush sb = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
                    g.FillRectangle(sb, g.ClipBounds);

                    // draw local rulers
                    using (Matrix localRulerMatrix = designStage.GetMatrix().SysMatrix())
                    {
                        localRulerMatrix.Multiply(rootM, MatrixOrder.Append);
                        PointF[] pts = designStage.Selected.GetTransformedPoints();
                        using (Matrix m = GetDynamicMatrix())
                        {
                            m.TransformPoints(pts);
                        }
                        localRulerSelection = pts.GetBounds();

                        rulers.DrawAt(g,
                            Point.Round(new PointF(localRulerMatrix.OffsetX, localRulerMatrix.OffsetY)),
                            localRulerSelection,
                            localRulerMatrix);
                    }
               }
            }
            else
            {
                g.FillRectangle(paperColor, vexObject.ViewPort.SysRectangle());
            }

            g.Transform = cameraMatrix;
            designStage.Draw(g);

            //g.SmoothingMode = SmoothingMode.Default;
            //g.InterpolationMode = InterpolationMode.Default;
            //g.TextRenderingHint = TextRenderingHint.SystemDefault;
            //g.CompositingQuality = CompositingQuality.Default;

            if (!Selection.IsEmpty)
            {
                g.Transform = identityMatrix;
                foreach (uint id in Selection.RelatedIds)
                {
                    if (designStage.Contains(id))
                    {
                        DesignInstance di = designStage[id];
                        PointF[] pts = di.GetTransformedPoints(cameraMatrix);

                        if (selIds.Contains(id))
                        {
                            // draw the (0,0) cross indicator
                            Point p = Point.Round(StageToCamera(designStage[id].Location.SysPointF()));
                            g.DrawLine(Pens.Black, new Point(p.X, p.Y - locationRadius), new Point(p.X, p.Y + locationRadius));
                            g.DrawLine(Pens.Black, new Point(p.X - locationRadius, p.Y), new Point(p.X + locationRadius, p.Y));

                            // and the blue outline
                            if (Selection.Count > 1)
                            {
                                g.DrawPolygon(Pens.LightBlue, pts);
                            }
                        }

                        if (di.AspectConstraint != Vex.AspectConstraint.None)
                        {
                            di.DrawAspectConstraints(g, di.AspectConstraint, pts.GetMidpointsAndCenter());
                        }

                        if (dragKind == DragKind.None)
                        {
                            PointF[] bondPoints = pts.GetMidpointsAndCenter();
                            transformHandles.graphicHolder = g;
                            transformHandles.DrawBondHandles(CurrentEditItem, bondPoints, id);
                            transformHandles.graphicHolder = null;
                        }
                    }
                }

                // draw transform handles
                g.Transform = identityMatrix;
                transformHandles.Draw(g);

                g.Transform = cameraMatrix;
                switch (dragKind)
                {
                    case DragKind.Object:

                        // dragged bond handles and selected/related outlines
                        Vex.Point offset = new Vex.Point(transformTranslateX, transformTranslateY);

                        g.Transform = identityMatrix;
                        Dictionary<uint, Vex.Rectangle> alreadyDiscovered = new Dictionary<uint, Vex.Rectangle>();
                        transformHandles.graphicHolder = g;
                        CurrentEditItem.BondStore.GetRelatedObjectTransforms(selIds, alreadyDiscovered, offset);

                        foreach (uint id in alreadyDiscovered.Keys)
                        {
                            DesignInstance di = designStage[id];
                            Vex.Point dif = new Vex.Point(di.Location.X - di.StrokeBounds.Left, di.Location.Y - di.StrokeBounds.Top);
                            Vex.Point tp = alreadyDiscovered[id].Point;
                            transformHandles.DrawOutlineAndBonds(id, tp.X + dif.X, tp.Y + dif.Y);
                        }
                        transformHandles.graphicHolder = null;


                        // g.Transform = cameraMatrix;
                        for (int i = 0; i < snapPoints.Length; i++)
                        {
                            Vex.Point snapPoint = snapPoints[i];
                            if (!snapPoint.IsEmpty)
                            {
                                g.Transform = identityMatrix;
                                g.SmoothingMode = SmoothingMode.None;
                                g.PixelOffsetMode = PixelOffsetMode.None;
                                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                                PointF sp = StageToCamera(snapPoint.SysPointF());
                                Bitmap icon = SnapStore.GetSnapIcon(snapTypes[i]);
                                g.DrawImageUnscaled(icon, 
                                    (int)sp.X - SnapStore.guideRadius, 
                                    (int)sp.Y - SnapStore.guideRadius);

                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                g.PixelOffsetMode = PixelOffsetMode.Default;
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            }
                        }

                        break;
                    case DragKind.Scale:
                        Selection.DrawScaled(g, transformScaleX, transformScaleY, transformCenter);
                        break;
                    case DragKind.Rotate:
                        Selection.DrawRotated(g, transformRotate, transformCenter);
                        break;
                }
            }

            g.Transform = identityMatrix;

            // draw bond lines
            transformHandles.DrawBondLines(g);
            
            // show rulers
            if (showRulers)
            {
                rulers.Draw(g);
            }
            
            // edit modes

            // name node
            if (isNameMode)
            {
            }

            //rectangle select
            if (dragKind == DragKind.RectangleSelect)
            {
                g.DrawRectangle(selectionRectPen, dragRect);
            }
            else if (dragKind == DragKind.AspectConstrain)
            {
                g.DrawLine(wideWhitePen, mouseDownOriginCamera, mouseCurrentCamera);
                ColorMask cm = (ColorMask)clickMap.GetColorValueAt(mouseCurrentCamera);
                if (cm.IsScale() || cm.IsRotate())
                {
                    g.DrawLine(allowedPen, mouseDownOriginCamera, mouseCurrentCamera);
                }
                else
                {
                    g.DrawLine(notAllowedPen, mouseDownOriginCamera, mouseCurrentCamera);
                }
            }
            else if (dragKind == DragKind.GuideHorizontal)
            {
                g.DrawLine(Guidelines.guidePen, dragRect.Location, new Point(dragRect.Right, dragRect.Top));
            }
            else if (dragKind == DragKind.GuideVertical)
            {
                g.DrawLine(Guidelines.guidePen, dragRect.Location, new Point(dragRect.Left, dragRect.Bottom));
            }
            else if (dragKind == DragKind.GuideRectangle)
            {
                PointF org = StageToCamera(Point.Empty);
                if (dragRect.Width < 0 || dragRect.Height < 0)
                {
                    g.DrawRectangle(Guidelines.guidePenError, dragRect.GetPositiveRect());
                }
                else
                {
                    g.DrawRectangle(Guidelines.guidePen, dragRect);
                }
            }

            // click map
            if (mouseDownOriginCamera == Point.Empty || clickMapNeedsUpdate)
            {
                g.Transform = cameraMatrix;
                UpdateClickMap(e);
                clickMapNeedsUpdate = false;
            }



            // debug mode
            if (keysDown.Contains(Keys.D))
            {
                isDebugMode = true;
                g.Transform = identityMatrix;
                g.DrawImage(clickMap.Bitmap, Point.Empty);

                foreach (uint id in designStage.InstanceIds)
                {
                    DesignInstance di = designStage[id];
                    PointF sloc = di.RotationCenter.Translate(di.Location).SysPointF();
                    PointF loc = StageToCamera(sloc);
                    designStage[id].DrawDebugInto(g, loc);
                }

                g.Transform = cameraMatrix;
                for (int i = 0; i < invalidAreas.Count; i++)
                {
                    g.DrawRectangle(invalidDebugPen, invalidAreas[i]);
                }
                invalidAreas.Clear();
            }
            else
            {
                isDebugMode = false;
            }

        }

        public Matrix GetCalculatedRootMatrix()
        {
            Matrix result = cameraMatrix.Clone();
            for (int i = 0; i < editStack.Count - 1; i++)
            {
                using (Matrix dim = editStack.ElementAt(i).GetSysMatrix().Clone())
                {
                    dim.Invert();
                    result.Multiply(dim, MatrixOrder.Prepend);
                }
            }
            return result;
        }
        public Matrix GetCurrentMatrix()
        {
            return designStage.GetSysMatrix();
        }


        #endregion

        #region IconInfo

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        private static Cursor CreateCursor(Bitmap bmp, Point hotspot)
        {
            IconInfo tmp = new IconInfo();
            GetIconInfo(bmp.GetHicon(), ref tmp);
            tmp.xHotspot = hotspot.X;
            tmp.yHotspot = hotspot.Y;
            tmp.fIcon = false;
            return new Cursor(CreateIconIndirect(ref tmp));
        }
        #endregion

        #region Load Save
        private void SetPaths(string fileFullPathAndName)
        {
            rootFolder = Path.GetDirectoryName(fileFullPathAndName);
            fileName = Path.GetFileName(fileFullPathAndName);
            vexObject.Name = Path.GetFileNameWithoutExtension(fileFullPathAndName);
            Name = vexObject.Name;
            this.Text = Name;
            Environment.CurrentDirectory = rootFolder;              
        }

        public bool Save(string fileFullPathAndName)
        {
            bool result = false;
            
            if (HasSaveableChanges)
            {
                SetPaths(fileFullPathAndName);

                if (!Directory.Exists(WorkingFolderFull))
                {
                    Directory.CreateDirectory(WorkingFolderFull);
                }

                File.WriteAllText(FileFullPathAndName, string.Empty);
                FileStream fs = new FileStream(FileFullPathAndName, FileMode.OpenOrCreate);
                // stage ID
                // Selection or top edited symbol
                // grouped elements
                // Library Path
                // Instances Path

                XmlTextWriter w = new XmlTextWriter(fs, System.Text.Encoding.UTF8);

                w.WriteStartDocument();

                //XMLNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                //XMLNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

                w.Formatting = Formatting.Indented;
                w.WriteStartElement("UIL");

                w.WriteElementString("Version", VersionNumber);
                w.WriteElementString("DefinitionsFolder", DefinitionsFolder);
                w.WriteElementString("InstancesFolder", InstancesFolder);
                w.WriteElementString("WorkingFolder", WorkingFolder);
                w.WriteElementString("RootId", root.InstanceHash.ToString());


                w.WriteStartElement("SelectedIds");
                uint[] selIds = Selection.SelectedIds;
                string s = "";
                string comma = "";
                foreach (var id in selIds)
                {
                    s += comma + id.ToString();
                    comma = ",";
                }
                w.WriteAttributeString("Ids", s);
                w.WriteEndElement();

                w.WriteEndElement();

                w.WriteEndDocument();

                w.Flush();

                fs.Close();

                instanceManager.Save(InstancesFolderFull);
                library.Save(DefinitionsFolderFull);
            }

            HasSaveableChanges = false;
            isNewFile = false;

            return result;
        }
        public bool LoadUIL(string fileFullPathAndName)
        {
            bool result = false;

            if (File.Exists(fileFullPathAndName))
            {
                SetPaths(fileFullPathAndName);

                uint[] selectedIds = new uint[] { };

                FileStream fs = new FileStream(FileFullPathAndName, FileMode.Open);

                XmlTextReader r = new XmlTextReader(fs);

                r.ReadStartElement("UIL");

                while (r.Read())
                {
                    if (r.IsStartElement())
                    {
                        switch (r.Name)
                        {
                            case "Version":
                                if (r.Read())
                                {
                                    versionNumber = r.Value.Trim();
                                }
                                break;
                            case "DefinitionsFolder":
                                if (r.Read())
                                {
                                    definitionsFolder = r.Value.Trim();
                                }
                                break;
                            case "InstancesFolder":
                                if (r.Read())
                                {
                                    instancesFolder = r.Value.Trim();
                                }
                                break;
                            case "RootId":
                                if (r.Read())
                                {
                                    RootId = uint.Parse(r.Value.Trim(), NumberStyles.Any);
                                }
                                break;
                            case "SelectedIds":
                                if (r.Read())
                                {
                                    string s = r.Value.Trim();
                                    if (s != "")
                                    {
                                        string[] idsSplit = s.Split(new char[] { ',' });
                                        selectedIds = new uint[idsSplit.Length];
                                        for (int i = 0; i < idsSplit.Length; i++)
                                        {
                                            selectedIds[i] = uint.Parse(idsSplit[i], NumberStyles.Any);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                fs.Close();

                library.LoadUIL(DefinitionsFolderFull);

                instanceManager.LoadUIL(InstancesFolderFull);

                // hack for now - needs all instances and defs to have been created
                // this adds actual instances to timeline (not just numeric refs).
                uint[] keys = Library.Keys;
                for (uint key = 0; key < keys.Length; key++)
                {
                    LibraryItem li = Library[keys[key]];
                    if (li.Definition is Vex.Timeline)
                    {
                        Vex.Timeline tl = (Vex.Timeline)li.Definition;
                        if (tl.InstanceIds != null)
                        {
                            for (int i = 0; i < tl.InstanceIds.Length; i++)
                            {
                                DesignInstance di = instanceManager[tl.InstanceIds[i]];
                                tl.AddInstance(di.Instance);
                            }
                        }
                    }
                }

                MainForm.CurrentLibraryView.LoadCurrentLibrary();
                SetRoot((DesignTimeline)instanceManager[RootId]);
                Selection.Set(selectedIds);

                pb.Invalidate();

                HasSaveableChanges = false;
                isNewFile = false;

                ResetZoom();
            }
            return result;
        }
        #endregion
    }
}
