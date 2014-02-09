using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.Drawing;

namespace DDW.Managers
{
    public class FontManager
    {        
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx
        (
            IntPtr pbFont,
            uint cbFont, 
            IntPtr pdv, 
            [System.Runtime.InteropServices.In] ref uint pcFonts
        );
        private static PrivateFontCollection fonts = new PrivateFontCollection();

        public static Font RulerFont;

        static FontManager()
        {
//          LoadFont(Properties.Resources.pf_ronda_seven);
//			RulerFont = new Font(fonts.Families[0], 6); 
			RulerFont = SystemFonts.SmallCaptionFont;
        }

        private static void LoadFont(byte[] res)
        {
            try
            {
                unsafe
                {
                    fixed (byte* pFontData = res)
                    {
                        uint dummy = 0;
                        fonts.AddMemoryFont((IntPtr)pFontData, res.Length);
                        AddFontMemResourceEx((IntPtr)pFontData, (uint)res.Length, IntPtr.Zero, ref dummy);
                    }
                }
            }
            catch
            {
				Console.WriteLine ("Font unable to load."); 
            }
        }


    }
}
