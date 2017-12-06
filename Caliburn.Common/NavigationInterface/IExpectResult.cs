using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModuleSystem.Common.NavigationInterface
{
    public interface IExpectResult<in TOutcome>
    {
        /// <summary>
        /// Called when the navigation request is marked as completed. The outcome of the navigation is supplied
        /// </summary>
        /// <param name="outcome">The outcome of the navigation</param>
        void AcceptResult(TOutcome outcome);
    }
}
