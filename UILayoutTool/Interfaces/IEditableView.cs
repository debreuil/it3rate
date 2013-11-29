using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DDW.Views
{
    interface IEditableView
    {
        bool HasEditFocus();        
        
        void OnKeyDown(object sender, KeyEventArgs e);
        void OnKeyPress(object sender, KeyPressEventArgs e);
        void OnKeyUp(object sender, KeyEventArgs e);
    }
}
