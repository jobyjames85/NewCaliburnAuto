using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModuleSystem.Common.NavigationInterface
{
    public interface IHasArgument<in TArg>
    {
        /// <summary>
        /// Called when argument is passed to the navigation handler
        /// </summary>
        /// <param name="arg">The argument</param>
        void AcceptArgument(TArg arg);
    }
}
