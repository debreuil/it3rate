using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using DDW.Views;
using DDW.Managers;
using DDW.Commands;
using DDW.Vex.Bonds;

namespace DDW.Tests
{
    public class CommandTests : AssertionHelper
    {
        MainForm mainForm;

        [SetUp]
        protected void SetUp()
        {
            mainForm = new MainForm();
            //SendKeys.Send("{ENTER}") 'for enter
            //SendKeys.Send("{%}") 'for Alt
            //SendKeys.Send("{^}") 'for Ctrl
            //SendKeys.Send("{+}") 'for shift
            //' Combination
            //SendKeys.Send("^(c)") 'for Ctrl-C

            //SendKeys.Send("^(n)"); // new doc
            mainForm.NewDocument(null, EventArgs.Empty);
        }

        [TearDown]
        protected void TearDown()
        {
            mainForm.CloseDocument(null, EventArgs.Empty);
        }

        [Test]
        public void GuideTests()
        {
            Guide guide1 = new Guide(new Vex.Point(20, 100), new Vex.Point(480, 100));
            Guide guide2 = new Guide(new Vex.Point(10, 10), new Vex.Point(400, 400));
            //AddGuideCommand agc = new AddGuideCommand(guide);
            MainForm.CurrentStage.AddGuide(guide1);
            MainForm.CurrentStage.AddGuide(guide2);
        }
    }
}
