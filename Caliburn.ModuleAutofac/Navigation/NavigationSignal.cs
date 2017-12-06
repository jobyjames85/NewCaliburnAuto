using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModuleSystem.Common.NavigationInterface;

namespace ModuleSystem.ModuleAutofac.Navigation
{
    public class NavigationSignal
    {
        public NavigationSignal(INavigationRequest request, NavigationStatus status)
        {
            this.Request = request;
            Status = status;
        }

        public INavigationRequest Request { get; private set; }

        public NavigationStatus Status { get; private set; }
    }

    public enum NavigationStatus
    {
        /// <summary>
        /// Navigation is about to begin
        /// </summary>
        Begin,
        /// <summary>
        /// Navigation has ended and new screen is now active in shell
        /// </summary>
        End
    }
}
