using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DDW.Display;
using DDW.Interfaces;
using System.IO;

namespace DDW.Views
{
    class FolderTreeNode : TreeNode, ILibraryTreeNode, IDataObject
	{
		public LibraryItem[] Items{ get { return new LibraryItem[]{ }; } }
        public string DisplayName { get; set; }
        public Type ContentsType { get; set; }
        public DateTime Date { get; set; }
        public bool CanUpdate { get { return DisplayName.ToUpper().EndsWith(".SWF"); } }
        public bool NeedsUpdate { get; set; }
        public Rectangle UpdateRectangle { get; set; }
        
        public FolderTreeNode(string folderName, Type type, int imageIndex, int selectedImageIndex) : base(folderName, imageIndex, selectedImageIndex)
        {
            this.DisplayName = folderName;
            this.ContentsType = type;
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
            MainForm.CurrentStage.Gdi.ClearBitmap(target);
        }


        // IDataObject
        public object GetData(Type format)
        {
            return ContentsType;
        }

        public object GetData(string format)
        {
            return null;
        }

        public object GetData(string format, bool autoConvert)
        {
            return null;
        }

        public bool GetDataPresent(Type format)
        {
            return format == ContentsType;
        }

        public bool GetDataPresent(string format)
        {
            string contentType = LibraryItem.folderFormatName;
            return format == contentType;
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            string contentType = LibraryItem.folderFormatName;
            return format == contentType;
        }

        public string[] GetFormats()
        {
            string contentType = LibraryItem.folderFormatName;
            return new string[] { contentType };
        }

        public string[] GetFormats(bool autoConvert)
        {
            string contentType = LibraryItem.folderFormatName;
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
