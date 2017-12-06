using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace ModuleSystem.Common.NavigationInterface
{
    public interface INavigator
    {
        /// <summary>
        /// Creates a navigation request for the current navigator
        /// </summary>
        /// <typeparam name="TModel">The view model where app needs to be navigated</typeparam>
        /// <returns>The navigation request</returns>
        INavigationRequest<TModel> Navigate<TModel>() where TModel : class, IScreen;
    }
}
