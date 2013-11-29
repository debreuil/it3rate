using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace DDW.Assets
{
    public unsafe class FastBitmap
    {
        private Bitmap bitmap;
        private Rectangle bounds;

        // three elements used for MakeGreyUnsafe
        private int width;
        private BitmapData bitmapData = null;
        private Byte* pBase = null;

        public FastBitmap(Bitmap bitmap)
        {
            //this.bitmap = new Bitmap(bitmap);
            this.bitmap = bitmap;
            bounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        }

        public FastBitmap(int width, int height)
        {
            this.bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bounds = new Rectangle(0, 0, width, height);
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }

        public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
        }
        public Graphics Graphics
        {
            get
            {
                return Graphics.FromImage(bitmap);
            }
        }

        private Point PixelSize
        {
            get
            {
                return new Point(bounds.Width, bounds.Height);
            }
        }

        public Color GetColorAt(Point p)
        {
            Color result = Color.White;

            if (p.X > 0 && p.X <= bitmap.Width && p.Y > 0 && p.Y < bitmap.Height)
            {
                lock (bitmap)
                {
                    LockPixels();
                    result = GetPixel(p.X, p.Y).Color;
                    UnlockPixels();
                }
            }
            return result;
        }
        public uint GetColorValueAt(Point p)
        {
            uint result = 0xFFFFFFFF;

            if (p.X > 0 && p.X <= bitmap.Width && p.Y > 0 && p.Y < bitmap.Height)
            {
                lock (bitmap)
                {
                    LockPixels();
                    result = GetPixel(p.X, p.Y).ArgbValue;
                    UnlockPixels();
                }
            }
            else
            {
            }
            return result;
        }
        public uint[] GetIdsInRect(Rectangle rect)
        {
            List<uint> ids = new List<uint>();
            int top = Math.Max(0, rect.Top);
            int left = Math.Max(0, rect.Left);
            int width = Math.Min(bitmap.Width, left + rect.Width);
            int height = Math.Min(bitmap.Height, top + rect.Height);

            lock (bitmap)
            {
                LockPixels();
                uint lastPixel = 0xFFFF;
                for (int y = top; y < height; y++)
                {
                    for (int x = left; x < width; x++)
                    {
                        uint id = (uint)GetPixel(x, y).RGBValue;
                        if (id != lastPixel && id < 0xFFFF)
                        {
                            lastPixel = id;
                            if (!ids.Contains(id))
                            {
                                ids.Add(id);
                            }
                        }
                    }
                }
                UnlockPixels();
            }

            return ids.ToArray();
        }

        public static Bitmap GetGhost(Bitmap bmp)
        {
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            lock (bmp)
            {
                FastBitmap bmpOrg = new FastBitmap(bmp);
                bmpOrg.LockPixels();
                FastBitmap fbmp = new FastBitmap(result);
                fbmp.LockPixels();

                int width = bmp.Width;
                int height = bmp.Height;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        PixelData c = bmpOrg.GetPixel(x, y);
                        if (c.alpha > 0)
                        {
                            PixelData* p = fbmp.PixelAt(x, y);
                            c.MakeGreyLuminous();
                            c.alpha = (byte)(c.alpha * .3f);

                            *p = c;
                        }
                    }
                }

                fbmp.UnlockPixels();
                bmpOrg.UnlockPixels();
            }
            return result;
        }

        public static Bitmap GetMask(Bitmap bmp, Color color)
        {
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            lock (bmp)
            {
                FastBitmap bmpOrg = new FastBitmap(bmp);
                bmpOrg.LockPixels();
                FastBitmap fbmp = new FastBitmap(result);
                fbmp.LockPixels();

                int width = bmp.Width;
                int height = bmp.Height;
                
                PixelData solid = new PixelData(color);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        PixelData c = bmpOrg.GetPixel(x, y);
                        if(c.alpha > 0)
                        {
                            PixelData* pixel = fbmp.PixelAt(x, y);
                            *pixel = solid;
                        }
                    }
                }

                fbmp.UnlockPixels();
                bmpOrg.UnlockPixels();
            }
            return result;
        }
        public void AddMaskRect(Color color, Rectangle rect)
        {
            lock (bitmap)
            {
                LockPixels();

                int orgX = (int)rect.Left;
                int orgY = (int)rect.Top;

                int startX = (int)Math.Abs(Math.Min(0, rect.Left));
                int startY = (int)Math.Abs(Math.Min(0, rect.Top));
                int width  = (int)Math.Min(bitmap.Width - rect.Left, rect.Width);
                int height = (int)Math.Min(bitmap.Height - rect.Top, rect.Height);

                PixelData solid = new PixelData(color);

                for (int x = startX; x < width; x++)
                {
                    for (int y = startY; y < height; y++)
                    {
                        PixelData* pixel = PixelAt(x + orgX, y + orgY);
                        *pixel = solid;
                    }
                }

                UnlockPixels();
            }
        }
        public Rectangle GetNonAlphaRectangle(Bitmap bmp)
        {
            int top = -1;
            int left = -1;
            int bottom = -1;
            int right = -1;

            lock (bitmap)
            {
                LockPixels();

                for (int y = 0; y < bounds.Height; y++)
                {
                    for (int x = 0; x < bounds.Width; x++)
                    {
                        if ((*PixelAt(x, y)).alpha != 0)
                        {
                            top = (int)(y / bounds.Width);
                            goto bottomTest;
                        }
                    }
                }

                bottomTest:
                for (int y = bounds.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < bounds.Width; x++)
                    {
                        if ((*PixelAt(x, y)).alpha != 0)
                        {
                            bottom = (int)(y / bounds.Width);
                            goto leftTest;
                        }
                    }
                }

                leftTest:
                for (int x = 0; x < bounds.Width; x++)
                {
                    for (int y = 0; y < bounds.Height; y++)
                    {
                        if ((*PixelAt(x, y)).alpha != 0)
                        {
                            left = x;
                            goto rightTest;
                        }
                    }
                }

                rightTest:
                for (int x = bounds.Width - 1; x <= 0; x--)
                {
                    for (int y = 0; y < bounds.Height; y++)
                    {
                        if ((*PixelAt(x, y)).alpha != 0)
                        {
                            right = x;
                            goto endTests;
                        }
                    }
                }

                endTests:
                UnlockPixels();
            }
            return new Rectangle(top, left, right - left, bottom - top);
        }

        public void ConvertToPremultiplied()
        {
            lock (bitmap)
            {
                LockPixels();

                GraphicsUnit unit = GraphicsUnit.Pixel;
                Rectangle bounds = Rectangle.Truncate(bitmap.GetBounds(ref unit));

                for (int x = 0; x < bounds.Width; x++)
                {
                    for (int y = 0; y < bounds.Height; y++)
                    {
                        PixelData c = *PixelAt(x, y);
                        float a = c.alpha / 255f;
                        PixelData preMult = new PixelData((int)(a * c.red), (int)(a * c.green), (int)(a * c.blue), c.alpha);
                        //PixelData preMult = new PixelData(128, c.alpha * c.green, c.alpha * c.blue, c.alpha);
                        PixelData* pixel = PixelAt(x, y);
                        *pixel = preMult;
                    }
                }

                UnlockPixels();
            }
        }
        public void MakeTransparent(float t)
        {
            if (t < 0 || t > 1)
            {
                throw new Exception("MakeTransparent only used to make bitmap more transparent. t must be between 0 and 1");
            }

            lock (bitmap)
            {
                LockPixels();

                GraphicsUnit unit = GraphicsUnit.Pixel;
                Rectangle bounds = Rectangle.Truncate(bitmap.GetBounds(ref unit));

                for (int x = 0; x < bounds.Width; x++)
                {
                    for (int y = 0; y < bounds.Height; y++)
                    {
                        PixelData c = *PixelAt(x, y);
                        float a = c.alpha * t;
                        PixelData alpha = new PixelData(c.red, c.green, c.blue, (int)a);
                        PixelData* pixel = PixelAt(x, y);
                        *pixel = alpha;
                    }
                }

                UnlockPixels();
            }
        }

        private PixelData GetPixel(int x, int y)
        {
            PixelData returnValue = *PixelAt(x, y);
            return returnValue;
        }

        private void SetPixel(int x, int y, PixelData c)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = c;
        }

        private PixelData* PixelAt(int x, int y)
        {
            return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
        }

        private void LockPixels()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
          (int)boundsF.Y,
          (int)boundsF.Width,
          (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length.
            width = (int)boundsF.Width * sizeof(PixelData);
            if (width % 4 != 0)
            {
                width = 4 * (width / 4 + 1);
            }
            bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            pBase = (Byte*)bitmapData.Scan0.ToPointer();
        }
        private void UnlockPixels()
        {
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }
    }



    public struct PixelData
    {
        public PixelData(Color col)
        {
            this.red = (byte)col.R;
            this.green = (byte)col.G;
            this.blue = (byte)col.B;
            this.alpha = (byte)col.A;
        }
        public PixelData(int r, int g, int b, int a)
        {
            this.red = (byte)r;
            this.green = (byte)g;
            this.blue = (byte)b;
            this.alpha = (byte)a;
        }
        public PixelData(byte r, byte g, byte b, byte a)
        {
            this.red = r;
            this.green = g;
            this.blue = b;
            this.alpha = a;
        }

        public uint ArgbValue
        {
            get
            {
                return (uint)(alpha * 0x1000000 + red * 0x10000 + green * 0x100 + blue);
            }
        }
        public int RGBValue
        {
            get
            {
                return red * 0x10000 + green * 0x100 + blue;
            }
        }
        public bool RGBEqual(PixelData c)
        {
            return (c.red == red) && (c.green == green) && (c.blue == blue);
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(alpha, red, green, blue);
            }
        }

        public void Interpolate(float t, PixelData pd)
        {
            float it;
            if (t > 0)
            {
                it = 1f - t;
            }
            else
            {
                it = t;
                t = 1f - it;
            }

            if (alpha > 0 || pd.alpha > 0)
            {
                this.red = (byte)((this.red * t) + (pd.red * it));
                this.green = (byte)((this.green * t) + (pd.green * it));
                this.blue = (byte)((this.blue * t) + (pd.blue * it));
                this.alpha = (byte)((this.alpha * t) + (pd.alpha * it));
            }
            else
            {
                this.alpha = (byte)((this.alpha * t) + (pd.alpha * it));
            }
        }
        public void MakeGreyscale()
        {
            byte gr = (byte)(red * 0.3f + green * 0.59f + blue * .11f);
            this.red = gr;
            this.green = gr;
            this.blue = gr;
        }
        public void MakeGreyLuminous()
        {
            float grf = red * 0.2f + green * 0.7f + blue * .1f;
            grf = Math.Min((grf - 128) * 3f + 128, 255);
            byte gr = (byte)Math.Max(0, grf);
            this.red = gr;
            this.green = gr;
            this.blue = gr;
        }

        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;
    }
}