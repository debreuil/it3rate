using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DDW.Display;
using DDW.Interfaces;
using System.IO;
using DDW.Commands;

namespace DDW.Views
{
    public class LibraryTreeView : TreeView
    {
        private ILibraryTreeNode updateHoverNode;
        private bool isMouseDown;

        public LibraryTreeView()
        {
            DrawMode = TreeViewDrawMode.OwnerDrawAll;

            this.MouseEnter += new EventHandler(LibraryTreeView_MouseEnter);
            this.MouseLeave += new EventHandler(LibraryTreeView_MouseLeave);
            this.MouseDown += new MouseEventHandler(LibraryTreeView_MouseDown);
            this.MouseUp += new MouseEventHandler(LibraryTreeView_MouseUp);

            this.MouseClick += new MouseEventHandler(LibraryTreeView_MouseClick);
        }

        public TreeNode GetNode(string folderPath, string name)
        {
            TreeNode result;

            string[] pathSegs = folderPath.Split(new char[]{Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
            List<string> segs = new List<string>(pathSegs);
            segs.Add(name);

            TreeNodeCollection curNodes = Nodes;
            do
            {
                result = null;
                foreach (TreeNode tn in curNodes)
                {
                    if (tn.Name == segs[0])
                    {
                        curNodes = tn.Nodes;
                        segs.RemoveAt(0);
                        result = tn;
                        break;
                    }
                }
            }
            while (result != null && segs.Count > 0);

            return result;
        }

        public static string GetNodePath(TreeNode node)
        {
            string path = node.Name;
            string sepChar = Path.DirectorySeparatorChar.ToString();
            while (node.Parent != null)
            {
                path = node.Parent.Name + sepChar + path;
                node = node.Parent;
            }
            return path;
        }

        void LibraryTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            // Update library element, not undoable so no command stack
            ILibraryTreeNode ltn = (ILibraryTreeNode)GetNodeAt(e.Location);
            if (ltn != null && ltn.NeedsUpdate && ltn.UpdateRectangle.Contains(e.Location))
            {
                ltn.NeedsUpdate = false;
                isMouseDown = false;
                Rectangle r = updateHoverNode.UpdateRectangle;
                updateHoverNode = null;
                InvalidateUpdate(r);

                // todo: move import off the UI thread
                string path = MainForm.CurrentStage.WorkingFolderFull + GetNodePath((TreeNode)ltn);
                ImportFileCommand ifc = new ImportFileCommand(path);
                ifc.Execute();
                ltn.ResetDate();

                MainForm.CurrentLibraryView.Invalidate();
                MainForm.CurrentStage.InvalidateAll();
            }
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);
            if (updateHoverNode != null && isMouseDown)
            {
                e.Cancel = true;
            }
        }

        void LibraryTreeView_MouseLeave(object sender, EventArgs e)
        {
            this.MouseMove -= new MouseEventHandler(LibraryTreeView_MouseMove);
            isMouseDown = false;
            if (updateHoverNode != null)
            {
                Rectangle r = updateHoverNode.UpdateRectangle;
                updateHoverNode = null;
                InvalidateUpdate(r);
            }
        }

        void LibraryTreeView_MouseEnter(object sender, EventArgs e)
        {
            this.MouseMove -= new MouseEventHandler(LibraryTreeView_MouseMove);
            this.MouseMove += new MouseEventHandler(LibraryTreeView_MouseMove);
        }

        void LibraryTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            ILibraryTreeNode ltn = (ILibraryTreeNode)GetNodeAt(e.Location);
            if (ltn != null && ltn.NeedsUpdate && ltn.UpdateRectangle.Contains(e.Location))
            {
                if (updateHoverNode != ltn)
                {
                    updateHoverNode = ltn;
                    InvalidateUpdate(ltn.UpdateRectangle);
                }
            }
            else
            {
                isMouseDown = false;
                if (updateHoverNode != null && updateHoverNode != ltn)
                {
                    Rectangle r = updateHoverNode.UpdateRectangle;
                    updateHoverNode = null;
                    InvalidateUpdate(r);
                }
            }
        }

        void InvalidateUpdate(Rectangle r)
        {
            if (r != Rectangle.Empty)
            {
                Invalidate(r);
            }
        }

        void LibraryTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
        }
        void LibraryTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;           
        }
        
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (e.Node is ILibraryTreeNode)
            {
                ILibraryTreeNode ltn = (ILibraryTreeNode)e.Node;
                if (ltn.NeedsUpdate)
                {
                    Font f = e.Node.NodeFont ?? this.Font;
                    SizeF textSize = e.Graphics.MeasureString(e.Node.Text, f);
                    Bitmap bmp;
                    if (updateHoverNode == e.Node)
                    {
                        if (isMouseDown)
                        {
                            bmp = Properties.Resources.update_d;
                        }
                        else
                        {
                            bmp = Properties.Resources.update_o;
                        }
                    }
                    else
                    {
                        bmp = Properties.Resources.update_n;
                    }
                    Rectangle rectUpdate = new Rectangle(new Point(e.Node.Bounds.X + (int)textSize.Width + 10, e.Bounds.Y), bmp.Size);
                    e.Graphics.DrawImageUnscaled(bmp, rectUpdate.Location);
                    ltn.UpdateRectangle = rectUpdate;
                }
            }
            e.DrawDefault = true;
        }
    }
}
