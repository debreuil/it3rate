using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.Interfaces
{
    public interface ICommand
    {
        void Execute();
        void UnExecute();
    }
}
