using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using DDW.Vex;

namespace DDW.VectorML
{
    public abstract class MLWriter : XmlTextWriter
    {
		protected string rootElement;

		public MLWriter(string rootElement, string filename, Encoding encoding) : base(filename, encoding)
		{
            this.rootElement = rootElement;

			this.Formatting = Formatting.Indented;
			this.Indentation = 2;
			this.Namespaces = true;
		}
        public MLWriter(string rootElement, TextWriter tw) : base(tw)
		{
            this.rootElement = rootElement;

			this.Formatting = Formatting.Indented;
			this.Indentation = 2;
			this.Namespaces = true;
		}

        public abstract void OpenHeaderTag(float width, float height, Color background);
        public abstract void OpenResourcesTag();
        public abstract void OpenSymbolDefTag(string key, int width, int height);
        public abstract void OpenRootTag();
        public abstract void OpenTimelineTag(string sprite);

        public void CloseTagAndFlush()
        {
            this.WriteEndElement();
            this.Flush();
        }
        public void CloseTag()
        {
            this.WriteEndElement();
        }

        public abstract void WriteVisualBrushRef(string key, string def);
        public abstract void WriteImageBrushRef(string key, Image img);
        public abstract void WriteMilliseconds(uint ms);
        public abstract void WriteMatrix(Matrix mx);
        public abstract void WriteColor(Color color);
        public abstract void WriteGradientColor(GradientFill gf);

        public void WriteFloat(float f)
        {
            this.WriteString(f.ToString("0.##"));
        }
        public void WritePoint(Point pt)
        {
            WriteFloat(pt.X);
            this.WriteString(",");
            WriteFloat(pt.Y);
            this.WriteString(" ");
        }
        public void WriteMoveTo(Point pt)
        {
            this.WriteString("M ");
            this.WritePoint(pt);
        }
        public void WriteLineTo(Point end)
        {
            this.WriteString("L ");
            this.WritePoint(end);
        }
        public void WriteCubicCurveTo(Point a0, Point a1, Point end)
        {
            this.WriteString("C ");
            this.WritePoint(a0);
            this.WritePoint(a1);
            this.WritePoint(end);
        }
        public void WriteQuadraticCurveTo(Point a0, Point end)
        {
            this.WriteString("Q ");
            this.WritePoint(a0);
            this.WritePoint(end);
        }
    }
}
