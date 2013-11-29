using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW;
using DDW.Managers;
using System.IO;
using DDW.Display;
using Vex = DDW.Vex;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UILayoutTests
{
    public abstract class BaseTest : AssertionHelper
    {
        protected MainForm mainForm;
        protected Library lib;
        protected LibraryItem[] lis;

        [SetUp]
        protected virtual void SetUp()
        {
            mainForm = new MainForm();
            mainForm.NewDocument(null, EventArgs.Empty);
            lib = MainForm.CurrentLibrary;
        }
        [TearDown]
        protected virtual void TearDown()
        {
            ClearLibrary();
            lib = null;
            lis = null;
        }

        protected LibraryItem[] CreateLibraryItems()
        {
            string testPath = Directory.GetCurrentDirectory() + @"\Images";
            string[] pngPaths = Directory.GetFiles(testPath, "*.png", SearchOption.AllDirectories);

            LibraryItem[] items = new LibraryItem[pngPaths.Length];
            LibraryItem li;
            for (int i = 0; i < pngPaths.Length; i++)
            {
                Vex.Image img = new Vex.Image(pngPaths[i], lib.NextLibraryId());
                li = new LibraryItem(MainForm.CurrentStage, img);
                items[i] = li;
            }
            return items;
        }
        protected LibraryItem[] PopulateLibrary()
        {
            LibraryItem[] lis = CreateLibraryItems();
            for (int i = 0; i < lis.Length; i++)
            {
                lib.AddLibraryItem(lis[i]);
            }
            return lis;
        }
        protected void ClearLibrary()
        {
            for (int i = 0; i < lis.Length; i++)
            {
                lib.RemoveLibraryItem(lis[i]);
            }
        }
    }
}
