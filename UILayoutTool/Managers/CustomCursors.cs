using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace DDW.Managers
{
    public static class CustomCursors
    {
        public static Cursor Arrow = Cursors.Arrow;
        public static Cursor Cross = Cursors.Cross;
        public static Cursor Finger = Cursors.Hand;
        public static Cursor Help = Cursors.Help;
        public static Cursor IBeam = Cursors.IBeam;
        public static Cursor No = Cursors.No;
        public static Cursor WaitCursor = Cursors.WaitCursor;


        public static Cursor ArrowDup = LoadCursorFromResource(Properties.Resources.arrowDup);
        public static Cursor ArrowMove = LoadCursorFromResource(Properties.Resources.arrowMove);
        public static Cursor RectSelect = LoadCursorFromResource(Properties.Resources.rectSelect);
        public static Cursor HandPan = LoadCursorFromResource(Properties.Resources.hand);
        public static Cursor HandClosed = LoadCursorFromResource(Properties.Resources.handClosed);

        public static Cursor MoveAllDir = LoadCursorFromResource(Properties.Resources.moveAllDir);
        public static Cursor ArrowMoveCenter = LoadCursorFromResource(Properties.Resources.arrowMoveCenter);
        public static Cursor Rotate = LoadCursorFromResource(Properties.Resources.rotate);
        public static Cursor StretchEW = LoadCursorFromResource(Properties.Resources.stretchEW);
        public static Cursor StretchNE_SW = LoadCursorFromResource(Properties.Resources.stretchNE_SW);
        public static Cursor StretchNS = LoadCursorFromResource(Properties.Resources.stretchNS);
        public static Cursor StretchNW_SE = LoadCursorFromResource(Properties.Resources.stretchNW_SE);
        public static Cursor ZoomIn = LoadCursorFromResource(Properties.Resources.zoomIn);
        public static Cursor ZoomNone = LoadCursorFromResource(Properties.Resources.zoomNone);
        public static Cursor ZoomOut = LoadCursorFromResource(Properties.Resources.zoomOut);

        [DllImport("User32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr LoadCursorFromFile(String str);

        public static Cursor LoadCursorFromResource(byte[] resource)
        {
            Cursor result;
            // Write a temp file here with the data in cursorStream
            string tempFile = "temp.cur";
            using (MemoryStream ms = new MemoryStream(resource))
            {
                File.WriteAllBytes(tempFile, ms.ToArray());
                result = new Cursor(LoadCursorFromFile(tempFile));
                File.Delete(tempFile);
            }

            return result;
        }
    }
}
