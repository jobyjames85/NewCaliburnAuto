using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleSystem.Common.NavigationInterface;
using Caliburn.Micro;

namespace ModuleSystem.ModuleAutofac.Navigation
{
    /// <summary>
    /// Default navigation request implementation
    /// </summary>
    /// <typeparam name="TViewModel">Type of view model</typeparam>
    public class NavigationRequest<TViewModel> : INavigationRequest<TViewModel>, IDisposable where TViewModel : class, IScreen
    {
        private List<Action<TViewModel>> _preprocessors;
        private List<Action<TViewModel>> _postprocessors;
        private Navigator _navigator;
        private IDisposable _lifetime;
        private TViewModel _model;
        private bool _isCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationRequest"/> class.
        /// </summary>
        /// <param name="navigator"></param>
        public NavigationRequest(Navigator navigator)
        {
            _preprocessors = new List<Action<TViewModel>>();
            _postprocessors = new List<Action<TViewModel>>();
            _navigator = navigator;
        }

        /// <summary>
        /// Registers pre processor actions that needs to be run on view model
        /// </summary>
        /// <param name="handler">Handler action</param>
        public void RegisterPreprocessor(Action<TViewModel> handler)
        {
            _preprocessors.Add(handler);
        }

        /// <summary>
        /// Registers post processing action that needs to be run after request has been concluded, ie after screen is closed.
        /// </summary>
        /// <param name="handler">Handler action</param>
        public void RegisterPostprocessor(Action<TViewModel> handler)
        {
            _preprocessors.Add(handler);
        }

        /// <summary>
        /// Initiates the navigation to the requested view model. Any previous active screen will be closed and its navigation request will be concluded
        /// </summary>
        /// <returns>The activated view model</returns>
        public async Task<TViewModel> Go()
        {
            return await Go(true);
        }

        /// <summary>
        /// Navigates to the requested view model
        /// </summary>
        /// <param name="endPrevious">If false, currently active screen is temporarly deactivated is automatically activated when this request is completed</param>
        /// <returns>The view model</returns>
        public async Task<TViewModel> Go(bool endPrevious)
        {
            _navigator.IssueSignal(this, NavigationStatus.Begin);
            var model = await ResolveAndProcess();
            await _navigator.Go(model, this, endPrevious);
            _navigator.IssueSignal(this, NavigationStatus.End);
            return model;
        }

        /// <summary>
        /// Navigates without concluding currently active screen
        /// </summary>
        public async Task Forward()
        {
            var model = await ResolveAndProcess();
            await _navigator.Go(model, this, false);
        }

        /// <summary>
        /// Concludes currently active screen
        /// </summary>
        public async Task Complete()
        {
            _navigator.Complete(this);
            await Task.Factory.StartNew(() =>
            {
                if (_model != null)
                {
                    foreach (var postprocess in _postprocessors)
                    {
                        postprocess(_model);
                    }
                }
            });

            _isCompleted = true;
        }

        /// <summary>
        /// Resolves the view model and runs the pre processors on it
        /// </summary>
        /// <returns>The view model object</returns>
        protected async Task<TViewModel> ResolveAndProcess()
        {
            await Execute.OnUIThreadAsync(() =>
            {
                _model = _navigator.Resolve<TViewModel>(this);
            });
            foreach (var processor in _preprocessors)
            {
                processor(_model);
            }
            return await Task.FromResult(_model);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lifetime"></param>
        internal void SetLifetime(IDisposable lifetime)
        {
            _lifetime = lifetime;
        }


        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_isCompleted)
                    this.Complete().Wait();

                if (this._model != null)
                {
                    this._model = null;
                }

                if (this._lifetime != null)
                {
                    this._lifetime.Dispose();
                    this._lifetime = null;
                }
            }
        }

        ~NavigationRequest()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }

}
