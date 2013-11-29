using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DDW.Enums;
using System.IO;
using DDW.Display;
using Vex = DDW.Vex;
using DDW.Interfaces;

namespace DDW.Views
{
    public partial class LibraryTreeNode : TreeNode, ILibraryTreeNode, IDataObject
    {
        public LibraryItem item { get; set; }

        public string DisplayName { get { return Path.GetFileNameWithoutExtension(item.Path); } }
        public Type ContentsType { get { return item.GetType(); } }
        public DateTime Date { get { return item.Date; } set { item.Date = value; } }
        public bool CanUpdate { get { return item.Definition is Vex.Image; } }
        public bool NeedsUpdate { get; set; }
        public Rectangle UpdateRectangle { get; set; }

        public LibraryTreeNode(LibraryItem libraryItem, int imageIndex, int selectedImageIndex) : base(libraryItem.Name, imageIndex, selectedImageIndex)
        {
            this.item = libraryItem;
            this.NeedsUpdate = false;
        }

        public void ResetDate()
        {
            string path = MainForm.CurrentStage.WorkingFolderFull + LibraryTreeView.GetNodePath(this);
            if (File.Exists(path))
            {
                Date = File.GetLastWriteTimeUtc(path);
            }
        }

        public void RenderThumbnail(Bitmap target)
        {
            item.CalculateBounds();
            MainForm.CurrentStage.Gdi.RenderInto(item.Definition, 0, target);
        }

        // IDataObject
        public object GetData(Type format)
        {
            return item;
        }

        public object GetData(string format)
        {
            return item;
        }

        public object GetData(string format, bool autoConvert)
        {
            return item;
        }

        public bool GetDataPresent(Type format)
        {
            return format == ContentsType;
        }

        public bool GetDataPresent(string format)
        {
            string contentType = LibraryItem.formatName;
            return format == contentType;
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            string contentType = LibraryItem.formatName;
            return format == contentType;
        }

        public string[] GetFormats()
        {
            string contentType = LibraryItem.formatName;
            return new string[] { contentType };
        }

        public string[] GetFormats(bool autoConvert)
        {
            string contentType = LibraryItem.formatName;
            return new string[] { contentType };
        }

        public void SetData(object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(Type format, object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(string format, object data)
        {
            throw new NotImplementedException();
        }

        public void SetData(string format, bool autoConvert, object data)
        {
            throw new NotImplementedException();
        }
    }
}
