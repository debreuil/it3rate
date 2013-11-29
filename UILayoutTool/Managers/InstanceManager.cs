using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using System.IO;
using System.Xml;
using System.Globalization;
using DDW.Views;
using DDW.Vex.Bonds;
using DDW.Utils;

namespace DDW.Managers
{
    public class InstanceManager
    {
        public StageView stage;
        public uint instanceIdCounter = 1;

        private Dictionary<uint, DesignInstance> items = new Dictionary<uint, DesignInstance>();
        private Dictionary<uint, DesignInstance> removedItems = new Dictionary<uint, DesignInstance>();

        private Dictionary<uint, string> removedPaths = new Dictionary<uint, string>();

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

        public InstanceManager()
        {
        }

        public int Count { get { return items.Count; } }
        public bool Contains(uint id)
        {
            return items.ContainsKey(id);
        }
        public DesignInstance this[uint id]
        {
            get
            {
                DesignInstance result = null;
                if (items.ContainsKey(id))
                {
                    result = items[id];
                }
                else if (IsRemovedInstance(id))
                {
                    result = removedItems[id];
                }
                return result;
            }
        }

        public bool IsRemovedInstance(uint id)
        {
            return removedItems.ContainsKey(id);
        }
        public uint AddInstance(DesignInstance item)
        {
            bool isNew = false;
            if (item.InstanceHash == 0)
            {
                isNew = true;
                item.InstanceHash = instanceIdCounter++;
            }
            uint hash = item.InstanceHash;

            if (removedItems.ContainsKey(hash))
            {
                CancelRemove(hash);
            }
            else if (!items.ContainsKey(hash))
            {
                items.Add(hash, item);
                if (removedPaths.ContainsKey(hash)) // shouldn't be needed
                {
                    removedPaths.Remove(hash);
                }
            }
            else if (!isNew) // recycling id
            {
                items[hash] = item;
                if (removedPaths.ContainsKey(hash)) // shouldn't be needed
                {
                    removedPaths.Remove(hash);
                }
            }
            else
            {
                throw new ArgumentException("duplicate instance id");
            }

            item.LibraryItem.UseCount++;
            if (hash > instanceIdCounter)
            {
                instanceIdCounter = hash;
            }

            return hash;
        }
        //public void AddUseCountById(uint id)
        //{
        //    if (items.ContainsKey(id))
        //    {
        //        items[id].LibraryItem.UseCount++;
        //    }
        //}
        public void RemoveInstance(DesignInstance item)
        {
            RemoveInstance(item.InstanceHash);
        }
        public void RemoveInstance(uint instanceHash)
        {
            if (items.ContainsKey(instanceHash))
            {
                items[instanceHash].LibraryItem.UseCount--;
                removedItems.Add(instanceHash, items[instanceHash]);
                removedPaths.Add(instanceHash, items[instanceHash].GetDataPath());

                items.Remove(instanceHash);
            }
        }
        public void CancelRemove(uint instanceHash)
        {
            if (IsRemovedInstance(instanceHash))
            {
                items.Add(instanceHash, removedItems[instanceHash]);
                items[instanceHash].LibraryItem.UseCount++;
                removedItems.Remove(instanceHash);
                removedPaths.Remove(instanceHash);
            }
        }

        //public UsageIdentifier[] FindAllUsagesOfDefinition(uint definitionId)
        //{
        //    List<UsageIdentifier> result = new List<UsageIdentifier>();
        //    foreach (uint key in items.Keys)
        //    {
        //        if (items[key] is DesignTimeline)
        //        {
        //            DesignTimeline dt = (DesignTimeline)items[key];
        //            foreach (uint instId in dt.InstanceIds)
        //            {
        //                if (items[instId].DefinitionId == definitionId)
        //                {
        //                    result.Add(new UsageIdentifier(dt.InstanceHash, instId));
        //                }
        //            }
        //        }
        //    }
        //    return result.ToArray();
        //}
        public void SortIndexesByLocation(List<uint> ids, ChainType chainType)
        {
            if (chainType.IsHorizontal())
            {
                ids.Sort((a, b) => this[a].StrokeBounds.Left.CompareTo(this[b].StrokeBounds.Left));
            }
            else
            {
                ids.Sort((a, b) => this[a].StrokeBounds.Top.CompareTo(this[b].StrokeBounds.Top));
            }
        }

        public bool Save(string folderPath)
        {
            bool result = false;

            DirectoryInfo dinf = Directory.CreateDirectory(folderPath);

            string libFileName = folderPath + Path.DirectorySeparatorChar + "Instances.xml";
            File.WriteAllText(libFileName, string.Empty);
            FileStream fs = new FileStream(libFileName, FileMode.OpenOrCreate);

            XmlTextWriter w = new XmlTextWriter(fs, System.Text.Encoding.UTF8);
            w.Formatting = Formatting.Indented;
            w.WriteStartElement("Instances");

            w.WriteElementString("InstanceIdCounter", stage.InstanceManager.instanceIdCounter.ToString());

            w.WriteStartElement("InstanceItems");

            foreach (uint key in items.Keys)
            {
                DesignInstance di = items[key];
                w.WriteStartElement("Item");
                w.WriteAttributeString("InstanceId", di.InstanceHash.ToString());
                w.WriteAttributeString("DefinitionId", di.DefinitionId.ToString());
                w.WriteAttributeString("ParentInstanceId", di.ParentInstanceId.ToString());
                w.WriteAttributeString("Type", di.GetType().ToString());
                w.WriteAttributeString("DataPath", di.GetDataPath());
                w.WriteEndElement();
            }
            w.WriteEndElement();

            w.WriteEndElement();
            w.Flush();
            fs.Close();

            // remove old defs
            foreach (uint key in removedPaths.Keys)
            {
                string path = folderPath + System.IO.Path.DirectorySeparatorChar + removedPaths[key];
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            removedPaths.Clear();

            // now write actual instance Items
            foreach (uint key in items.Keys)
            {
                DesignInstance di = items[key];
                if (di.HasSaveableChanges)
                {
                    di.Save(folderPath);
                }
            }

            hasSaveableChanges = false;

            return result;
        }

        public bool LoadUIL(string folderPath)
        {
            bool result = false;

            List<string> dataPaths = new List<string>();
            List<string> types = new List<string>();

            string libFileName = folderPath + "Instances.xml";
            FileStream fs = new FileStream(libFileName, FileMode.Open);
            XmlTextReader r = new XmlTextReader(fs);
            r.WhitespaceHandling = WhitespaceHandling.None;
            r.ReadStartElement("Instances");

            do
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "InstanceIdCounter":
                            if (r.Read())
                            {
                                stage.InstanceManager.instanceIdCounter = uint.Parse(r.Value.Trim(), NumberStyles.Any);
                                r.Read();
                            }
                            break;

                        case "InstanceItems":
                            while (r.Read())
                            {
                                if (r.IsStartElement() && r.Name == "Item")
                                {
                                    dataPaths.Add(r.GetAttribute("DataPath"));
                                    types.Add(r.GetAttribute("Type"));
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

            // read actual DesignInstance Items
            for (int i = 0; i < dataPaths.Count; i++)
            {
                string dp = folderPath + dataPaths[i];
                string type = types[i];
                DesignInstance.LoadFromPath(stage, type, dp);
            }

            hasSaveableChanges = false;

            return result;
        }
    }
}
