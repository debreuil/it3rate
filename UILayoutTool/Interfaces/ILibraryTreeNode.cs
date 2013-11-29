using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DDW.Interfaces
{
    public interface ILibraryTreeNode : IDataObject
    {
        string DisplayName { get;}
        Type ContentsType { get; }
        DateTime Date { get; set; }
        bool CanUpdate { get; }
        bool NeedsUpdate { get; set;  }
        void ResetDate();
        Rectangle UpdateRectangle { get; set; }

        void RenderThumbnail(Bitmap target);
    }
}
