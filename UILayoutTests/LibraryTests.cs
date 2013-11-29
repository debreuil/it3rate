using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DDW;
using DDW.Managers;
using System.IO;
using DDW.Display;
using Vex = DDW.Vex;

namespace UILayoutTests
{
    [TestFixture]
    public class LibraryTests : BaseTest
    {
		[SetUp]
		protected override void SetUp()
        {
            base.SetUp();
            lis = CreateLibraryItems(); 
        }
        [TearDown]
        protected override void TearDown() 
        {
            base.TearDown();
        }

        [Test]
        public void CountAssets()
        {
            Assert.AreEqual(1, lib.Count); // root
            lib.AddLibraryItem(lis[0]);
            lib.AddLibraryItem(lis[1]);
            Assert.AreEqual(3, lib.Count);
            lib.AddLibraryItem(lis[2]);
            Assert.AreEqual(4, lib.Count);
            lib.RemoveLibraryItem(lis[2]);
            lib.RemoveLibraryItem(lis[2]);
            Assert.AreEqual(3, lib.Count);
            lib.RemoveLibraryItem(lis[1]);
            Assert.AreEqual(2, lib.Count);
            lib.RemoveLibraryItem(lis[0]);
            lib.RemoveLibraryItem(lis[0]);
            Assert.AreEqual(1, lib.Count);
        }

    }
}
