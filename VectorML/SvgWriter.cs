using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using DDW.Vex;

namespace DDW.VectorML
{
    public class SvgWriter : MLWriter
    {
        
		public SvgWriter(string filename, Encoding encoding) : base("svg", filename, encoding)
		{
			this.Formatting = Formatting.Indented;
			this.Indentation = 2;
			this.Namespaces = true;
		}
        public SvgWriter(TextWriter tw) : base("svg", tw)
		{
			this.Formatting = Formatting.Indented;
			this.Indentation = 2;
			this.Namespaces = true;
		}


        public override void OpenHeaderTag(float width, float height, Color background)
        {
            //<svg version="1.1"
            // baseProfile="full"
            // xmlns="http://www.w3.org/2000/svg"
            // xmlns:xlink="http://www.w3.org/1999/xlink"
            // xmlns:ev="http://www.w3.org/2001/xml-events">
            
              // <style type="text/css"><![CDATA[
              // ]]></style>

            rootElement = "svg";

            this.WriteStartElement(rootElement);

            this.WriteAttributeString("version", @"1.1");
            this.WriteAttributeString("baseProfile", @"full");
            this.WriteAttributeString("xmlns", @"http://www.w3.org/2000/svg");
            this.WriteAttributeString("xmlns:xlink", @"http://www.w3.org/1999/xlink");
            this.WriteAttributeString("xmlns:ev", @"http://www.w3.org/2001/xml-events");

            this.WriteStartAttribute("width");
            this.WriteValue(width);
            this.WriteEndAttribute();

            this.WriteStartAttribute("height");
            this.WriteValue(height);
            this.WriteEndAttribute();

            this.WriteStartElement("style");

            this.WriteStartAttribute("type");
            this.WriteValue("text/css");
            this.WriteEndAttribute();

            this.WriteCData(@"path { symbol {overflow:visible}  path {stroke-width:0.3 stroke-linejoin:round stroke-linecap:round }");

            this.WriteEndElement();
        }
        public override void OpenResourcesTag()
        {
            this.WriteStartElement("defs");
        }
        public override void OpenSymbolDefTag(string key, int width, int height)
        {
            // <Canvas Width="10" Height="10" RenderTransformOrigin="0,0" Canvas.Left="0" Canvas.Top="0">
            this.WriteStartElement("symbol");

            this.WriteStartAttribute("id");
            this.WriteValue(key);
            this.WriteEndAttribute();

            this.WriteStartAttribute("width");
            this.WriteValue(width);
            this.WriteEndAttribute();

            this.WriteStartAttribute("height");
            this.WriteValue(height);
            this.WriteEndAttribute();

            //this.WriteAttributeString("RenderTransformOrigin", "0,0");
            this.WriteAttributeString("x", "0");
            this.WriteAttributeString("y", "0");

            this.WriteStartAttribute("overflow");
            this.WriteValue("visible");
            this.WriteEndAttribute();
        }
        public override void OpenRootTag()
        {
            this.WriteStartElement("g");

            this.WriteStartAttribute("id");
            this.WriteValue("_root");
            this.WriteEndAttribute();
        }
        public override void OpenTimelineTag(string sprite)
        {
            this.WriteStartElement("g");

            this.WriteStartAttribute("id");
            this.WriteValue(sprite);
            this.WriteEndAttribute();
        }




        public override void WriteVisualBrushRef(string key, string def)
        {
            // <VisualBrush x:Key="VisualBrush2" Visual="{Binding Source={StaticResource def2}}"/>
            this.WriteStartElement("VisualBrush");

            this.WriteStartAttribute("x:Key");
            this.WriteValue(key);
            this.WriteEndAttribute();

            this.WriteStartAttribute("Visual");
            this.WriteValue("{Binding Source={StaticResource " + def + "}}");
            this.WriteEndAttribute();

            this.WriteEndElement();
        }
        public override void WriteImageBrushRef(string key, Image img)
        {
            // <image xlink:href="items/car.svg" width="30" height="30" />
            this.WriteStartElement("image");

            this.WriteStartAttribute("id");
            this.WriteValue(key);
            this.WriteEndAttribute();

            this.WriteStartAttribute("xlink:href");
            this.WriteValue(img.Path);
            this.WriteEndAttribute();

            this.WriteStartAttribute("width");
            this.WriteValue(img.StrokeBounds.Width);
            this.WriteEndAttribute();

            this.WriteStartAttribute("height");
            this.WriteValue(img.StrokeBounds.Height);
            this.WriteEndAttribute();

            this.WriteEndElement();
        }
        public override void WriteMilliseconds(uint ms)
        {
            // "0:0:0.25"   h/m/s/fs
            uint h = (uint)(ms / (60 * 60 * 1000));
            ms -= h * (60 * 60 * 1000);
            uint m = (uint)(ms / (60 * 1000));
            ms -= m * (60 * 1000);
            float s = ms / 1000F;
            this.WriteString(h + ":" + m + ":" + s);
        }
        public override void WriteMatrix(Matrix mx)
        {
            this.WriteString(
                mx.ScaleX + " " +
                mx.Rotate0 + " " +
                mx.Rotate1 + " " +
                mx.ScaleY + " " +
                mx.TranslateX + " " +
                mx.TranslateY);
        }
        public override void WriteColor(Color color)
        {
            if (color.A == 255)
            {
                this.WriteString("#" + color.RGB.ToString("X6"));
            }
            else
            {
                this.WriteString("rgba(" + color.R + "," + color.G + "," + color.B + "," + color.A + ")");
            }
        }
        public override void WriteGradientColor(GradientFill gf)
        {
            //this.WriteString("#" + gf.Color.ARGB.ToString("X8"));
        }
    }
}
