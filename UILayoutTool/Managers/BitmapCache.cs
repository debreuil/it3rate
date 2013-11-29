using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vex = DDW.Vex;
using DDW.Gdi;
using System.IO;
using DDW.Views;

namespace DDW.Managers
{
    public class BitmapCache
    {
        StageView stage;
        InstanceManager instMgr = MainForm.CurrentInstanceManager;
        Dictionary<long, Bitmap> cache = new Dictionary<long, Bitmap>();

        public BitmapCache(StageView stage)
        {
            this.stage = stage;
        }

        public Vex.Rectangle GetBounds(Vex.IDefinition def)
        {
            Bitmap bmp = GetBitmap(def);
            return new Vex.Rectangle(0, 0, bmp.Width, bmp.Height);
        }

        public static Image LoadImageNoLock(string path)
        {
            using (var ms = new MemoryStream(File.ReadAllBytes(path)))
            {
                return Image.FromStream(ms);
            }
        }
        public Bitmap GetBitmap(Vex.IDefinition def)
        {
            Bitmap result = null;

            long key = GetDefinitionIdentifier(def);
            if (cache.ContainsKey(key))
            {
                result = cache[key];
            }
            else if (def.Path != null)// if (def.StrokeBounds == Vex.Rectangle.Empty) // from file, not swf
            {
                string path = File.Exists(def.Path) ? def.Path : stage.DefinitionsFolderFull + def.Path;
                Image img = LoadImageNoLock(path);
                result = new Bitmap(img);                
                img.Dispose();

                result.SetResolution(96, 96);
                def.StrokeBounds = new Vex.Rectangle(0, 0, result.Width, result.Height);
                result.Tag = key;
                cache[key] = result;
            }

            return result;
        }

        public void AddBitmap(Vex.IDefinition def, Bitmap bmp)
        {
            long key = GetDefinitionIdentifier(def);
            bmp.Tag = key;
            cache[key] = bmp;
        }
        public void RemoveBitmap(Vex.IDefinition def)
        {
            long key = GetDefinitionIdentifier(def);
            if (cache.ContainsKey(key))
            {
                Bitmap bmp = cache[key];
                cache.Remove(key);
                bmp.Dispose();
            }
        }

        private long GetDefinitionIdentifier(Vex.IDefinition def)
        {
            return def.Id;
        }
    }
}
