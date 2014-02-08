using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DDW.Display;
using DDW.Managers;
using DDW.Utils;
using DDW.Vex;
using DDW.Commands;
using DDW.Interfaces;

namespace DDW.Views
{
    public partial class PropertyBar : Form, IEditableView
    {
        private float originalValue;
        private float newValue;

        public PropertyBar()
        {
            InitializeComponent();

            this.SizeChanged += new EventHandler(PropertyBar_SizeChanged);

            // enter key runs command
            this.KeyDown += new KeyEventHandler(PropertyBar_KeyDown);
            // click off sets value
            this.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            definitionLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            instanceLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            xLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            yLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            widthLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            heightLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            xScaleLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            yScaleLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            rotationLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            shearLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            stageColorLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            stageSizeLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            stageSizeXLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            sepColor.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            sepDebug.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            sepScale.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            debugBoundsLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            debugDepthLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            debugMatrixLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            debugOffsetLabel.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            tDebugBounds.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            tDebugDepth.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            tDebugMatrix.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            tDebugOffset.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            tDefinitionName.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);
            tShear.MouseDown += new MouseEventHandler(PropertyBar_MouseDown);

            tbX.GotFocus += new EventHandler(tb_GotFocus);
            tbY.GotFocus += new EventHandler(tb_GotFocus);
            tbWidth.GotFocus += new EventHandler(tb_GotFocus);
            tbHeight.GotFocus += new EventHandler(tb_GotFocus);
            tbXScale.GotFocus += new EventHandler(tb_GotFocus);
            tbYScale.GotFocus += new EventHandler(tb_GotFocus);
            tbRotation.GotFocus += new EventHandler(tb_GotFocus);

            tbInstance.LostFocus += new EventHandler(tbInstance_LostFocus); 
            tbX.LostFocus += new EventHandler(tbX_LostFocus);
            tbY.LostFocus += new EventHandler(tbY_LostFocus);
            tbWidth.LostFocus += new EventHandler(tbWidth_LostFocus);
            tbHeight.LostFocus += new EventHandler(tbHeight_LostFocus);
            tbXScale.LostFocus += new EventHandler(tbXScale_LostFocus);
            tbYScale.LostFocus += new EventHandler(tbYScale_LostFocus);
            tbRotation.LostFocus += new EventHandler(tbRotation_LostFocus);

            Reset();
            PopulateData(null);
        }

        public bool HasEditFocus()
        {
            return this.Focused;
        }

        public void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e) { }
        public void OnKeyDown(object sender, KeyEventArgs e) { }
        public void OnKeyPress(object sender, KeyPressEventArgs e) { }
        public void OnKeyUp(object sender, KeyEventArgs e) { }

        void PropertyBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.Focus();
                ((Control)sender).Focus();
            }
        }

        void PropertyBar_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
        }
        
        void PropertyBar_SizeChanged(object sender, EventArgs e)
        {
            Reset();
        }

        void tb_GotFocus(object sender, EventArgs e)
        {
            float.TryParse(((TextBox)sender).Text, out originalValue);
        }

        void tbInstance_LostFocus(object sender, EventArgs e)
        {
            if (MainForm.CurrentStage != null && MainForm.CurrentStage.CurrentEditItem != null && MainForm.CurrentStage.Selection.Count == 1)
            {
                DesignInstance di = MainForm.CurrentInstanceManager[MainForm.CurrentStage.Selection.SelectedIds[0]];
                di.InstanceName = tbInstance.Text;
            }
        }

        private bool NeedsCommand(TextBox tb)        
        {
            bool result = false;
            if (MainForm.CurrentStage != null)
            {
                result = float.TryParse(tb.Text, out newValue);
                if (result && (newValue - originalValue != 0))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        void tbX_LostFocus(object sender, EventArgs e)
        {
            if (NeedsCommand((TextBox)sender))
            {
                MoveSelectionCommand command = new MoveSelectionCommand(new Vex.Point(newValue - originalValue, 0));
                DoCommand(command);
            }
        }
        void tbY_LostFocus(object sender, EventArgs e)
        {
            if (NeedsCommand((TextBox)sender))
            {
                MoveSelectionCommand command = new MoveSelectionCommand(new Vex.Point(0, newValue - originalValue));
                DoCommand(command);
            }
        }
        void tbWidth_LostFocus(object sender, EventArgs e)
        {
            if (NeedsCommand((TextBox)sender))
            {
                float scaleDif = newValue / originalValue;
                ScaleTransformCommand command = new ScaleTransformCommand(scaleDif, 1, MainForm.CurrentStage.Selection.GlobalRotationCenter);
                DoCommand(command);
            }
        }
        void tbHeight_LostFocus(object sender, EventArgs e)
        {
            if (NeedsCommand((TextBox)sender))
            {
                float scaleDif = newValue / originalValue;
                ScaleTransformCommand command = new ScaleTransformCommand(1, scaleDif, MainForm.CurrentStage.Selection.GlobalRotationCenter);
                DoCommand(command);
            }
        }
        void tbXScale_LostFocus(object sender, EventArgs e)
        {
            if (NeedsCommand((TextBox)sender))
            {
                float scaleDif = newValue / originalValue;
                ScaleTransformCommand command = new ScaleTransformCommand(scaleDif, 1, MainForm.CurrentStage.Selection.GlobalRotationCenter);
                DoCommand(command);
            }
        }
        void tbYScale_LostFocus(object sender, EventArgs e)
        {
            if (NeedsCommand((TextBox)sender))
            {
                float scaleDif = newValue / originalValue;
                ScaleTransformCommand command = new ScaleTransformCommand(1, scaleDif, MainForm.CurrentStage.Selection.GlobalRotationCenter);
                DoCommand(command);
            }
        }
        void tbRotation_LostFocus(object sender, EventArgs e)
        {
            if (NeedsCommand((TextBox)sender))
            {
                float rotDif = (newValue - originalValue) % 360f;
                RotateTransformCommand command = new RotateTransformCommand(rotDif);
                DoCommand(command);
            }
        }

        private void DoCommand(ICommand command)
        {
            MainForm.CurrentStage.CommandStack.Do(command);
            PopulateData(MainForm.CurrentStage.Selection);
        }

        public void PopulateData(InstanceGroup selection)
        {
            if (selection == null || selection.Count == 0)
            {
                tDefinitionName.Text = "";
                tbInstance.Text = "";
                tbX.Text = "";
                tbY.Text = "";
                tbWidth.Text = "";
                tbHeight.Text = "";
                tbXScale.Text = "";
                tbYScale.Text = "";
                tbRotation.Text = "";
                tShear.Text = "";
            }
            else
            {
                uint[] ids = selection.SelectedIds;
                MatrixComponents mc = selection.TransformMatrix.VexMatrix().GetMatrixComponents();
                if (selection.Count == 1)
                {
                    DesignInstance di = MainForm.CurrentInstanceManager[ids[0]];
                    tDefinitionName.Text = di.Definition.Name;
                    tbInstance.Text = di.InstanceName;
                }
                else
                {
                }

                tbX.Text = selection.Location.X.ToString("0.##");
                tbY.Text = selection.Location.Y.ToString("0.##");
                tbWidth.Text = (selection.UntransformedBounds.Width * mc.ScaleX).ToString("0.##");
                tbHeight.Text = (selection.UntransformedBounds.Height * mc.ScaleY).ToString("0.##");

                tbXScale.Text = mc.ScaleX.ToString("0.##");
                tbYScale.Text = mc.ScaleY.ToString("0.##");
                tbRotation.Text = mc.Rotation.ToString("0.##");
                tShear.Text = mc.Shear.ToString("0.##");
            }

            if (MainForm.CurrentStage != null)
            {
                btStageColor.BackColor = MainForm.CurrentStage.BackgroundColor.SysColor();
                tbStageWidth.Text = MainForm.CurrentStage.Width.ToString();
                tbStageHeight.Text = MainForm.CurrentStage.Height.ToString();
            }
        }

        void Reset()
        {
            if (this.Size.Width > tDebugMatrix.Location.X + tDebugMatrix.Size.Width)
            {
                tDebugMatrix.Visible = true;
                debugMatrixLabel.Visible = true;
                tDebugDepth.Visible = true;
                debugDepthLabel.Visible = true;
            }
            else
            {
                tDebugMatrix.Visible = false;
                debugMatrixLabel.Visible = false;
                tDebugDepth.Visible = false;
                debugDepthLabel.Visible = false;
            }

            if (this.Size.Width > tDebugBounds.Location.X + tDebugBounds.Size.Width)
            {
                tDebugBounds.Visible = true;
                debugBoundsLabel.Visible = true;
                tDebugOffset.Visible = true;
                debugOffsetLabel.Visible = true;
                sepDebug.Visible = true;
            }
            else
            {
                tDebugBounds.Visible = false;
                debugBoundsLabel.Visible = false;
                tDebugOffset.Visible = false;
                debugOffsetLabel.Visible = false;
                sepDebug.Visible = false;
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {                
                this.SizeChanged -= new EventHandler(PropertyBar_SizeChanged); 
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void btStageColor_Click(object sender, EventArgs e)
        {
            if (MainForm.CurrentStage != null)
            {
                ColorDialog cd = new ColorDialog();
                cd.Color = MainForm.CurrentStage.BackgroundColor.SysColor();
                cd.ShowDialog();

                MainForm.CurrentStage.BackgroundColor = cd.Color.VexColor();
                btStageColor.BackColor = MainForm.CurrentStage.BackgroundColor.SysColor();
                MainForm.CurrentStage.InvalidateAll();
            }
        }

    }
}
