using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ModuleSystem.Common.NavigationInterface
{
    /// <summary>
    /// Encapsulates a navigation request
    /// </summary>
    public interface INavigationRequest : IDisposable
    {
        /// <summary>
        /// Concludes a navigation
        /// </summary>
        Task Complete();
    }

    /// <summary>
    /// Encapsulates a navigation request
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface INavigationRequest<TViewModel> : INavigationRequest where TViewModel : IScreen
    {
        /// <summary>
        /// Navigates to the requested view model
        /// </summary>
        /// <returns>The view model</returns>
        Task<TViewModel> Go();

        /// <summary>
        /// Navigates to the requested view model
        /// </summary>
        /// <param name="endPrevious">If false, currently active screen is temporarly deactivated is automatically activated when this request is completed</param>
        /// <returns>The view model</returns>
        Task<TViewModel> Go(bool endPrevious);
    }
}
