using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using DDW.Vex;
using System.Xml;
using System.IO;
using System.Globalization;
using DDW.Views;

namespace DDW.Managers
{
    public class Library
    {
        public StageView stage;
        private Dictionary<uint, LibraryItem> items = new Dictionary<uint, LibraryItem>();

        private Dictionary<uint, string> addedPaths = new Dictionary<uint, string>();
        private Dictionary<uint, string> removedPaths = new Dictionary<uint, string>();

        private uint definitionIdCounter = 1;
        private uint unnamedSymbolCounter = 1;

        private bool hasSaveableChanges = false;
        public bool HasSaveableChanges
        {
            get
            {
                if (!hasSaveableChanges)
                {
                    foreach (uint id in items.Keys)
                    {
                        if (items[id].HasSaveableChanges)
                        {
                            hasSaveableChanges = true;
                            break;
                        }
                    }
                }
                return hasSaveableChanges;
            }
            set
            {
                hasSaveableChanges = value;
                foreach (uint id in items.Keys)
                {
                    items[id].HasSaveableChanges = value;
                }
            }
        }

        public Library()
        {
        }

        public int Count { get { return items.Count; } }
        public uint[] Keys { get { return items.Keys.ToArray(); } }
        public bool Contains(uint id)
        {
            return items.ContainsKey(id);
        }

        public bool HasPath(string libraryPath, string fileName)
        {
            bool result = false;
            foreach (uint key in items.Keys)
            {
                LibraryItem li = items[key];
                if (li.LibraryPath != null && li.LibraryPath == libraryPath && li.Name == fileName)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public LibraryItem GetLibraryItem(string libraryPath, string definitionName)
        {
            LibraryItem result = null;
            foreach (uint key in items.Keys)
            {
                LibraryItem li = items[key];
                if (li.Name != null && li.LibraryPath != null && li.Name == definitionName && li.LibraryPath == libraryPath)
                {
                    result = li;
                    break;
                }
            }
            return result;
        }
        public LibraryItem GetByOriginalSourceId(uint originalSourceId)
        {
            LibraryItem result = null;
            foreach (uint key in items.Keys)
            {
                LibraryItem li = items[key];
                if (li.OriginalSourceId == originalSourceId)
                {
                    result = li;
                    break;
                }
            }            
            return result;
        }
        public LibraryItem this[uint index]
        {
            get
            {
                return items[index];
            }
        }

        public uint NextLibraryId()
        {
            return definitionIdCounter++;
        }

        public string GetNextDefaultName()
        {
            return "Symbol" + unnamedSymbolCounter++;
        }

        public void AddLibraryItem(LibraryItem item)
        {
            if (!items.ContainsKey(item.DefinitionId))
            {
                uint id = item.DefinitionId;
                items.Add(id, item);
                stage.vexObject.Definitions.Add(id, item.Definition);

                addedPaths.Add(id, item.GetDataPath());
                if (removedPaths.ContainsKey(id))
                {
                    removedPaths.Remove(id);
                }
            }
            else
            {
                items[item.DefinitionId] = item;
                stage.vexObject.Definitions[item.DefinitionId] = item.Definition;
            }
        }
        public void RemoveLibraryItem(LibraryItem item)
        {
            if (items.ContainsKey(item.DefinitionId))
            {
                uint id = item.DefinitionId;
                items.Remove(id);
                stage.vexObject.Definitions.Remove(id);
                removedPaths.Add(id, item.GetDataPath());
                if (addedPaths.ContainsKey(id))
                {
                    addedPaths.Remove(id);
                }
            }
        }

        public uint[] FindAllUsagesOfDefinition(uint definitionId)
        {
            List<uint> result = new List<uint>();
            foreach (uint key in items.Keys)
            {
                IDefinition def = items[key].Definition;
                if (def is Vex.Timeline)
                {
                    foreach (uint instId in ((Vex.Timeline)def).InstanceIds)
                    {
                        if (definitionId == stage.InstanceManager[instId].DefinitionId)// && !result.Contains(instId))
                        {
                            result.Add(instId);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        public static string ImportableFileExtensions = "*.UIL;*.SWF;*.BMP;*.JPG;*.GIF;*.PNG";
        public static bool HasImportableFile(string[] paths)
        {
            bool result = false;
            for (int i = 0; i < paths.Length; i++)
            {
                string ext = Path.GetExtension(paths[i]).ToUpperInvariant();
                if (ImportableFileExtensions.IndexOf(ext) > -1)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool Save(string folderPath)
        {
            bool result = false;
            Directory.CreateDirectory(folderPath);
            // next ID
            // root file
            // unnamed symbol counter (or calc)

            string libFileName = folderPath + Path.DirectorySeparatorChar + "Library.xml";
            File.WriteAllText(libFileName, string.Empty);
            FileStream fs = new FileStream(libFileName, FileMode.OpenOrCreate);

            XmlTextWriter w = new XmlTextWriter(fs, System.Text.Encoding.UTF8);
            w.Formatting = Formatting.Indented;
            w.WriteStartElement("Library");

            w.WriteElementString("LibraryIdCounter", definitionIdCounter.ToString());

            w.WriteStartElement("LibraryItems");

            foreach(uint key in items.Keys)
            {
                LibraryItem li = items[key];
                w.WriteStartElement("Item");
                w.WriteAttributeString("Id", li.Definition.Id.ToString());
                w.WriteAttributeString("Type", li.Definition.GetType().ToString());
                w.WriteAttributeString("Date", li.Date.ToString("u"));

                if (li.OriginalSourceId != 0)
                {
                    w.WriteAttributeString("OriginalSourceId", li.OriginalSourceId.ToString());
                }

                if (li.Definition.Name != null && li.Definition.Name != "")
                {
                    w.WriteAttributeString("Name", li.Definition.Name);
                }

                if (li.LibraryPath != null && li.LibraryPath != "")
                {
                    w.WriteAttributeString("LibraryPath", li.LibraryPath);
                }

                string dp = li.GetDataPath();
                if (dp != null && dp != "")
                {
                    w.WriteAttributeString("DataPath", dp);
                }

                w.WriteEndElement();
            }
            w.WriteEndElement();

            w.WriteEndElement();
            w.Flush();
            fs.Close();

            // remove old defs
            foreach (uint key in removedPaths.Keys)
            {
                string[] paths = removedPaths[key].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string subPath in paths)
                {
                    string path = folderPath + System.IO.Path.DirectorySeparatorChar + subPath;
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }                     
                }
            }
            addedPaths.Clear();
            removedPaths.Clear();

            // now write actual library Items
            foreach (uint key in items.Keys)
            {
                LibraryItem li = items[key];
                if (li.HasSaveableChanges)
                {
                    li.Save(folderPath);
                }
            }

            hasSaveableChanges = false;

            return result;
        }
        public bool LoadUIL(string folderPath)
        {
            bool result = false;
            
            List<uint> ids = new List<uint>();
            List<string> types = new List<string>();
            List<DateTime> dates = new List<DateTime>();

            List<uint> originalSourceIds = new List<uint>();
            List<string> names = new List<string>();
            List<string> libraryPaths = new List<string>();
            List<string> dataPaths = new List<string>();

            string libFileName = folderPath + "Library.xml";
            FileStream fs = new FileStream(libFileName, FileMode.Open);            
            XmlTextReader r = new XmlTextReader(fs);
            r.WhitespaceHandling = WhitespaceHandling.None;
            r.ReadStartElement("Library");

            do
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "LibraryIdCounter":
                            if (r.Read())
                            {
                                definitionIdCounter = uint.Parse(r.Value.Trim(), NumberStyles.Any);
                                r.Read();
                            }
                            break;

                        case "LibraryItems":
                            while (r.Read())
                            {
                                if (r.IsStartElement() && r.Name == "Item")
                                {
                                    ids.Add(uint.Parse(r.GetAttribute("Id"), CultureInfo.InvariantCulture));
                                    types.Add(r.GetAttribute("Type"));
                                    dates.Add(DateTime.ParseExact(r.GetAttribute("Date"), "u", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));

                                    string osid = r.GetAttribute("OriginalSourceId");
                                    if (osid != null)
                                    {
                                        uint orgId;
                                        uint.TryParse(osid, out orgId);
                                        originalSourceIds.Add(orgId);
                                    }
                                    else
                                    {
                                        originalSourceIds.Add(0);
                                    }

                                    names.Add(r.GetAttribute("Name"));
                                    dataPaths.Add(r.GetAttribute("DataPath"));
                                    libraryPaths.Add(r.GetAttribute("LibraryPath"));
                                }
                            }
                            break;

                        default:
                            r.Read();
                            break;
                    }
                }
            }
            while (r.Read());

            r.Close();
            fs.Close();

            // read actual library Items
            for (int i = 0; i < ids.Count; i++)
            {
                string dp = folderPath + dataPaths[i];
                string type = types[i];

                LibraryItem li = LibraryItem.LoadFromPath(stage, type, dp);
                if (li != null) // todo: account for missing files
                {
                    li.Name = names[i];
                    li.OriginalSourceId = originalSourceIds[i];
                    li.Date = dates[i];
                    li.LibraryPath = libraryPaths[i];

                    AddLibraryItem(li);
                    li.HasSaveableChanges = false;
                }
            }

            hasSaveableChanges = false;

            return result;
        }
    }
}
