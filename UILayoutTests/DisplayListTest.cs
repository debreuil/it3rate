using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW;
using DDW.Managers;
using DDW.Display;
using NUnit.Framework;
using DDW.Vex;
using DDW.Commands;

namespace UILayoutTests
{
    [TestFixture]
    public class DisplayListTest : BaseTest
    {
        DesignTimeline dl;

		[SetUp]
		protected override void SetUp() 
		{
            base.SetUp();
            lis = PopulateLibrary();

            dl = MainForm.CurrentStage.Root;
        }
        [TearDown]
        protected override void TearDown() 
        {
            ClearLibrary(); 
        }

        [Test]
        public void AddRemoveTest()
        {
            DesignInstance si0 = dl.Add(lis[0].DefinitionId, new Point(10, 100));
            DesignInstance si1 = dl.Add(lis[1].DefinitionId, new Point(20, 200));
            DesignInstance si2 = dl.Add(lis[2].DefinitionId, new Point(30, 300));
            DesignInstance si3 = dl.Add(lis[2].DefinitionId, new Point(40, 400));
            Assert.AreEqual(4, dl.Count);
            dl.Remove(si2.InstanceHash);
            Assert.AreEqual(3, dl.Count);

            uint[] defIds = new uint[] { lis[0].DefinitionId, lis[1].DefinitionId, lis[2].DefinitionId };
            dl.AddRange(new uint[] { }, new Point[]{});
            uint[] instIds = dl.AddRange(defIds, new Point[] { Point.Empty, Point.Empty, Point.Empty });
            Assert.AreEqual(6, dl.Count);

            dl.RemoveInstancesById(instIds);
            dl.RemoveInstancesById(new uint[] { si0.InstanceHash });
            dl.RemoveInstancesById(new uint[] { si1.InstanceHash });
            dl.RemoveInstancesById(new uint[] { si3.InstanceHash });
            Assert.AreEqual(0, dl.Count);
        }
        [Test]
        public void SelectionTest()
        {
            DesignInstance si0 = dl.Add(lis[0].DefinitionId, new Point(10, 100));
            DesignInstance si1 = dl.Add(lis[1].DefinitionId, new Point(20, 200));
            DesignInstance si2 = dl.Add(lis[2].DefinitionId, new Point(30, 300));
            DesignInstance si3 = dl.Add(lis[2].DefinitionId, new Point(40, 400));

            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { }, SelectionModifier.SetSelection));
            Assert.AreEqual(0, dl.Selected.Count);
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { si0.InstanceHash }, SelectionModifier.SetSelection));
            Assert.AreEqual(1, dl.Selected.Count);
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { si1.InstanceHash }, SelectionModifier.AddToSelection));
            Assert.AreEqual(2, dl.Selected.Count);
            // no dups
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { si1.InstanceHash }, SelectionModifier.AddToSelection));
            Assert.AreEqual(2, dl.Selected.Count);

            Assert.True(si0.IsSelected);
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { si0.InstanceHash }, SelectionModifier.SubtractFromSelection));
            Assert.False(si0.IsSelected);
            Assert.AreEqual(1, dl.Selected.Count);

            Assert.True(si1.IsSelected);

            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] {}, SelectionModifier.SetSelection));
            Assert.False(si1.IsSelected);
            Assert.False(si2.IsSelected);
            Assert.AreEqual(0, dl.Selected.Count);

            // delete selections when instances are deleted
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(dl.InstanceIds, SelectionModifier.SetSelection));
            Assert.AreEqual(4, dl.Selected.Count);
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { si0.InstanceHash }, SelectionModifier.SubtractFromSelection));
            Assert.AreEqual(3, dl.Selected.Count);
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { }, SelectionModifier.SetSelection));
            Assert.AreEqual(0, dl.Selected.Count);
        }

        [Test]
        public void UseCountTests()
        {
            DesignInstance si0a = dl.Add(lis[0].DefinitionId, new Point(10, 100));
            Assert.AreEqual(1, lis[0].UseCount);
            DesignInstance si0b = dl.Add(lis[0].DefinitionId, new Point(10, 100));
            DesignInstance si0c = dl.Add(lis[0].DefinitionId, new Point(10, 100));
            Assert.AreEqual(3, lis[0].UseCount);
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(dl.InstanceIds, SelectionModifier.SetSelection));
            Assert.AreEqual(3, lis[0].UseCount);
            dl.Remove(si0a.InstanceHash);
            Assert.AreEqual(2, lis[0].UseCount);
            dl.RemoveInstancesById(new uint[] { si0b.InstanceHash, si0c.InstanceHash });
            Assert.AreEqual(0, lis[0].UseCount);

            dl.AddInstancesById(new uint[] { si0a.InstanceHash });
            Assert.AreEqual(1, lis[0].UseCount);

            DesignInstance si1 = dl.Add(lis[1].DefinitionId, new Point(20, 200));
            dl.AddInstancesById(new uint[] { si1.InstanceHash, si1.InstanceHash }); // should ignore multiple adds
            Assert.AreEqual(1, lis[0].UseCount);
            Assert.AreEqual(1, lis[1].UseCount);
            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(new uint[] { }, SelectionModifier.SetSelection));
            dl.Remove(si1.InstanceHash);
            dl.Remove(si0a.InstanceHash);
            Assert.AreEqual(0, lis[0].UseCount);
            Assert.AreEqual(0, lis[1].UseCount);

        }
    }
}
