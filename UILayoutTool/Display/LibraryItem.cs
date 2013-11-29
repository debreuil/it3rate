using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Enums;
using System.Drawing;
using Vex = DDW.Vex;
using DDW.Managers;
using DDW.Utils;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DDW.Views;
using System.Globalization;

namespace DDW.Display
{

    public class LibraryItem
    {
        public static string formatName = "LibraryItem";
        public static string folderFormatName = "LibraryFolderItem";

        private StageView stage;

        private Vex.IDefinition definition;
        public Vex.IDefinition Definition { get { return definition; } set { definition = value; } }
        public uint DefinitionId { get { return definition.Id; } }

        public string Name
        {
            get { return definition.Name; }
            set
            {
                if(definition.Name != value)
                {
                    if (definition.Name != null)
                    {
                        HasSaveableChanges = true;
                    }
                    definition.Name = value; 
                }
            }
        }
        private DateTime date;
        public Vex.Size Size { get { return (definition == null) ? Vex.Size.Empty : definition.StrokeBounds.Size; } }
        public string Path { get { return (definition == null) ? "" : definition.Path; } }
        public string WorkingPath { get { return (definition == null) ? "" : definition.WorkingPath; } }

        private string libraryPath = "";
        public string LibraryPath { get { return libraryPath; } set { libraryPath = value; } }
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (date != DateTime.MinValue)
                {
                    HasSaveableChanges = true;
                }
                date = value; 
            }
        }
        public bool HasSaveableChanges 
        {
            get { return definition.HasSaveableChanges; }
            set 
            { 
                definition.HasSaveableChanges = value;
            } 
        }
        public int UseCount { get; set; }

        // identifier from orginal file (eg swf)
        public uint OriginalSourceId { get; set; }

        public LibraryItem(StageView stage, Vex.IDefinition definition, uint id)
        {
            this.stage = stage;
            this.definition = definition;
            this.Date = (definition.Path == null) ? DateTime.Now.ToUniversalTime() : File.GetLastAccessTimeUtc(definition.Path);
            this.Name = (definition.Name == null) ? null : definition.Name;
            definition.Id = id;
        }
        public LibraryItem(StageView stage, Vex.IDefinition definition)
        {
            this.stage = stage;
            this.definition = definition;
            this.Date = (definition.Path == null) ? DateTime.Now.ToUniversalTime() : File.GetLastAccessTimeUtc(definition.Path);
            this.Name = (definition.Name == null) ? null : definition.Name;
        }

        public Bitmap GetScaledImage(System.Drawing.Drawing2D.Matrix m)
        {
            return stage.Gdi.RenderFirstFrame(definition, m);
        }

        public void EnsureImageLoaded()
        {
            // images have no bounds until loaded
            if (definition.StrokeBounds.Width == 0)
            {
                stage.Gdi.RenderFirstFrame(definition);
            }
        }

        public Bitmap GetImage(Vex.Transform transform)
        {
            return GetImage(transform.Matrix);
        }
        public Bitmap GetImage(Vex.Matrix m)
        {
            Bitmap result = stage.Gdi.RenderFirstFrame(definition, m);
            return result;
        }

        public void CalculateBounds()
        {
            if (definition is Vex.Timeline)
            {
                Vex.Timeline vt = (Vex.Timeline)definition;

                Vex.Rectangle bounds = Vex.Rectangle.Empty;
                float top = int.MaxValue;
                float left = int.MaxValue;
                uint[] instances = vt.GetInstanceIds();
                foreach (uint id in instances)
                {
                    DesignInstance inst = MainForm.CurrentInstanceManager[id];
                    if (inst != null && inst.Instance.GetTransformAtTime(0) != null)
                    {
                        if (bounds.IsEmpty)
                        {
                            bounds = inst.StrokeBounds;
                        }
                        else
                        {
                            bounds = inst.StrokeBounds.Union(bounds);// Rectangle.Union(bounds, inst.Bounds);
                        }

                        left = Math.Min(left, inst.Location.X);
                        top = Math.Min(top, inst.Location.Y);
                    }

                }

                this.Definition.StrokeBounds = bounds;
            }
        }

        public string GetDataPath()
        {
            string result = null;
            if (Definition is Vex.Timeline)
            {
                result = "tl_" + definition.Id + ".xml";
            }
            else if (Definition is Vex.Symbol)
            {
                result = "sym_" + definition.Id + ".xml";
            }
            else if (Definition is Vex.Image)
            {
                result = "img_" + definition.Id + ".xml";
            }
            return result;
        }

        public bool Save(string folderPath)
        {
            bool result = false;
            // Save using ID
            // save def using name
            // LibraryId
            // Date
            // Path in tree
            // path to Def
            // save def on path

            FileStream fs;

            string dataFileName = folderPath + System.IO.Path.DirectorySeparatorChar + GetDataPath();

            if (Definition is Vex.Timeline)
            {
                //save definition
                File.WriteAllText(dataFileName, string.Empty);
                fs = new FileStream(dataFileName, FileMode.OpenOrCreate);

                XmlSerializer xs = new XmlSerializer(this.Definition.GetType());
                xs.Serialize(fs, this.Definition);
                fs.Close();
            }
            else if (Definition is Vex.Image)
            {
                Vex.Image vi = (Vex.Image)Definition;
                string fileName = System.IO.Path.GetFileName(vi.Path);
                if (fileName != vi.Path)
                {
                    stage.BitmapCache.RemoveBitmap(vi);
                    File.Copy(vi.Path, folderPath + System.IO.Path.DirectorySeparatorChar + fileName, true);
                    vi.Path = fileName;
                }

                dataFileName = folderPath + System.IO.Path.DirectorySeparatorChar + GetDataPath();
                File.WriteAllText(dataFileName, string.Empty);
                fs = new FileStream(dataFileName, FileMode.OpenOrCreate);
                XmlSerializer xs = new XmlSerializer(this.Definition.GetType());
                xs.Serialize(fs, this.Definition);
                fs.Close();
            }
            else if (Definition is Vex.Symbol)
            {
                //save definition
                File.WriteAllText(dataFileName, string.Empty);
                fs = new FileStream(dataFileName, FileMode.OpenOrCreate);

                XmlSerializer xs = GetShapeSerializer();
                xs.Serialize(fs, this.Definition);
                fs.Close();
            }

            HasSaveableChanges = false;

            return result;
        }
        public static LibraryItem LoadFromPath(StageView stage, string type, string dataPath)
        {
            LibraryItem result = null;

            if (File.Exists(dataPath)) // todo: account for missing files
            {
                FileStream fs = new FileStream(dataPath, FileMode.Open);
                XmlSerializer xs = null;
                switch (type)
                {
                    case "DDW.Vex.Timeline":
                        xs = new XmlSerializer(typeof(Vex.Timeline));
                        break;
                    case "DDW.Vex.Symbol":
                        xs = GetShapeSerializer();
                        break;
                    case "DDW.Vex.Image":
                        xs = new XmlSerializer(typeof(Vex.Image));
                        break;
                }

                if (xs != null)
                {
                    Vex.IDefinition def = (Vex.IDefinition)xs.Deserialize(fs);
                    result = new LibraryItem(stage, def);
                    result.date = File.GetLastWriteTimeUtc(dataPath);
                    result.HasSaveableChanges = false;
                }

                fs.Close();
            }
            return result;
        }
        private static XmlSerializer GetShapeSerializer()
        {
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
            XmlElementAttribute attr;

            XmlAttributes fillAttributes = new XmlAttributes();
            attr = new XmlElementAttribute(typeof(DDW.Vex.SolidFill));
            attr.ElementName = "SolidFill";
            fillAttributes.XmlElements.Add(attr);
            attr = new XmlElementAttribute(typeof(DDW.Vex.GradientFill));
            attr.ElementName = "GradientFill";
            fillAttributes.XmlElements.Add(attr);
            attr = new XmlElementAttribute(typeof(DDW.Vex.ImageFill));
            attr.ElementName = "ImageFill";
            fillAttributes.XmlElements.Add(attr);
            attrOverrides.Add(typeof(DDW.Vex.Shape), "Fill", fillAttributes);            

            XmlAttributes strokeAttributes = new XmlAttributes();
            attr = new XmlElementAttribute(typeof(DDW.Vex.SolidStroke));
            attr.ElementName = "SolidStroke";
            strokeAttributes.XmlElements.Add(attr);
            attrOverrides.Add(typeof(DDW.Vex.Shape), "Stroke", strokeAttributes);

            XmlSerializer xs = new XmlSerializer(typeof(Vex.Symbol), attrOverrides);

            return xs;
        }
    }
}
