namespace DDW.Views
{
    partial class LibraryView
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
            this.label1 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.symbolTree = new DDW.Views.LibraryTreeView();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = " ";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 153);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(217, 3);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox.Location = new System.Drawing.Point(0, 13);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(217, 140);
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // symbolTree
            // 
            this.symbolTree.AllowDrop = true;
            this.symbolTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.symbolTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.symbolTree.Location = new System.Drawing.Point(0, 156);
            this.symbolTree.Name = "symbolTree";
            this.symbolTree.Size = new System.Drawing.Size(217, 374);
            this.symbolTree.TabIndex = 6;
            // 
            // LibraryView
            // 
            this.ClientSize = new System.Drawing.Size(217, 530);
            this.Controls.Add(this.symbolTree);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "LibraryView";
            this.Text = "LibraryView";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Splitter splitter1;
        private LibraryTreeView symbolTree;
    }
}