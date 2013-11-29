using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Interfaces;
using DDW.Views;
using DDW.Display;
using DDW.Managers;
using DDW.Utils;
using System.Drawing.Drawing2D;

namespace DDW.Commands
{
    public class BreakApartCommand : ICommand, ISaveableCommand
    {
        int[] prevDepths;
        uint[] prevInstances;
        uint[] prevSelection;
        uint[] newInstances;

        public BreakApartCommand()
        {
        }

        public void Execute()
        {
            StageView stage = MainForm.CurrentStage;
            InstanceGroup sel = stage.Selection;
            InstanceManager instMgr = stage.InstanceManager;

            prevSelection = sel.IdsByDepth;
            List<uint> addedInstances = new List<uint>();

            List<uint> newSelection = new List<uint>();
            List<uint> toBreakApart = new List<uint>();
            List<int> originalDepths = new List<int>();
            for (int i = 0; i < prevSelection.Length; i++)
            {
                DesignInstance di = instMgr[prevSelection[i]];
                if (CanBreakApart(di))
                {
                    toBreakApart.Add(prevSelection[i]);
                    originalDepths.Add(di.Depth);
                }
                else
                {
                    newSelection.Add(prevSelection[i]);
                }
            }

            prevInstances = toBreakApart.ToArray();
            prevDepths = originalDepths.ToArray();

            sel.Clear();
            for (int i = 0; i < toBreakApart.Count; i++)
            {
                uint id = toBreakApart[i];
                DesignInstance di = instMgr[id];
                int orgDepth = di.Depth;
                Matrix orgMatrix = di.GetSysMatrix();

                Vex.Point offset = di.Location;
                uint[] instances = ((DesignTimeline)di).InstanceIds;

                for (int j = instances.Length - 1; j >= 0; j--)
                {
                    DesignInstance orgInstance = instMgr[instances[j]];
                    uint subLibId = orgInstance.LibraryItem.DefinitionId;
                    DesignInstance inst = stage.AddInstance(subLibId, Vex.Point.Zero);
                    stage.CurrentEditItem.ChangeDepth(inst.Depth, orgDepth);

                    using (Matrix m = orgInstance.GetSysMatrix().Clone())
                    {
                        m.Multiply(orgMatrix, MatrixOrder.Append);
                        stage.SetDesignInstanceMatrix(inst, m.VexMatrix());
                    }

                    addedInstances.Add(inst.InstanceHash);
                    newSelection.Add(inst.InstanceHash);
                }
                stage.RemoveInstancesById(new uint[]{id});

            }

            newInstances = addedInstances.ToArray();
            sel.Set(newSelection.ToArray());
            stage.ResetTransformHandles();
        }

        public void UnExecute()
        {
            StageView stage = MainForm.CurrentStage;
            //DDW.Display.DesignTimeline curEditItem = stage.CurrentEditItem;
            InstanceGroup sel = stage.Selection;
            stage.RemoveInstancesById(newInstances);

            for (int i = 0; i < prevInstances.Length; i++ )
            {
                stage.InsertExistingInstance(prevDepths[i], MainForm.CurrentInstanceManager[prevInstances[i]]);
            }

            sel.Set(prevSelection);
            stage.ResetTransformHandles();
            stage.InvalidateSelection();
        }

        public static bool CanBreakApart(uint[] instanceIds)
        {
            bool result = false;
            for (int i = 0; i < instanceIds.Length; i++)
            {
                if (CanBreakApart(MainForm.CurrentInstanceManager[instanceIds[i]]))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        private static bool CanBreakApart(DesignInstance di)
        {
            bool result = false;
            if (di is DesignTimeline)
            {
                DesignTimeline dt = (DesignTimeline)di;
                if (dt.Count > 1 || (dt.Count == 1 && MainForm.CurrentInstanceManager[dt.InstanceIds[0]].Definition is Vex.Timeline))
                {
                    result = true;
                }
            }
            return result;
        }

    }
}
