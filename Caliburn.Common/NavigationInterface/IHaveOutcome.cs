using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModuleSystem.Common.NavigationInterface
{
    public interface IHaveOutcome<out TOutcome>
    {
        TOutcome GetResult();
    }
}
