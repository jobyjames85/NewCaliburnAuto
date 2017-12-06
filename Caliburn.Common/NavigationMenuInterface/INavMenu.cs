using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleSystem.Common.NavigationMenuInterface
{
    public interface INavMenu
    {
        string Name { get; }
        string Description { get; }
        Task Activate();
    }
}
