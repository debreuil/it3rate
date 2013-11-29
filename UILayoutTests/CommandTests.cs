using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DDW.Managers;
using DDW.Display;
using UILayoutTests;
using DDW.Commands;
using DDW.Views;
using DDW.Vex;
using DDW;

[assembly: RequiresSTA]

namespace UILayoutTests
{
    [TestFixture]
    public class CommandTests : BaseTest
    {
        CommandStack commandStack;
        DesignTimeline dl;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            lis = PopulateLibrary();
            commandStack = MainForm.CurrentStage.CommandStack;
            dl = MainForm.CurrentStage.Root;
        }
        [TearDown]
        protected override void TearDown()
        {
            ClearLibrary();
        }

        [Test]
        public void CreateObjectsTest()
        {
            Assert.True(lis.Length > 2); 
            commandStack.Do(new AddInstancesCommand(new uint[] { lis[0].DefinitionId }, new Point[] { new Point(5, 5) }));
            Assert.AreEqual(1, dl.Count);
            commandStack.Undo();
            Assert.AreEqual(0, dl.Count);
            commandStack.Undo();
            Assert.AreEqual(0, dl.Count);
            commandStack.Redo();
            Assert.AreEqual(1, dl.Count);
            commandStack.Redo();
            Assert.AreEqual(1, dl.Count);

            commandStack.Do(new AddInstancesCommand(new uint[] { lis[1].DefinitionId, lis[2].DefinitionId, lis[0].DefinitionId },
                                                    new Point[] { new Point(15, 15), new Point(135, 15), new Point(195, 15)} ));
            Assert.AreEqual(4, dl.Count);
            commandStack.Undo();
            Assert.AreEqual(1, dl.Count);
            commandStack.Redo();
            Assert.AreEqual(4, dl.Count);

            MainForm.CurrentStage.CommandStack.Do(new SelectInstancesCommand(dl.InstanceIds, SelectionModifier.SetSelection));
            Assert.AreEqual(4, dl.Selected.Count);

            commandStack.Do(new DeleteInstancesCommand(dl.InstanceIds));
            Assert.AreEqual(0, dl.Count);
            Assert.AreEqual(0, dl.Selected.Count);
            commandStack.Undo();
            Assert.AreEqual(4, dl.Selected.Count);
            commandStack.Redo();
            Assert.AreEqual(0, dl.Selected.Count);
        }
    }
}
