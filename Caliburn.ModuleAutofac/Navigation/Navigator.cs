using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ModuleSystem.Common.NavigationInterface;
using Caliburn.Micro;
using ModuleSystem.ModuleAutofac.Configurations;
using ModuleSystem.ModuleAutofac.Shell.ViewModels;

namespace ModuleSystem.ModuleAutofac.Navigation
{
    [SingleInstance]
    public class Navigator : INavigator
    {
        private Lazy<ShellViewModel> _shell;
        private Autofac.ILifetimeScope _containerContext;
        private Stack<INavigationRequest> _navigationStack;

        private object _locker = new object();
        private IEventAggregator _eventHub;

        /// <summary>
        /// Intilizes a new instance of the <see cref="Navigator"/> class.
        /// </summary>
        /// <param name="shell">Shel view model factory</param>
        /// <param name="containerContext"></param>
        public Navigator(Func<ShellViewModel> shell, ILifetimeScope containerContext, IEventAggregator eventHub)
        {
            _navigationStack = new Stack<INavigationRequest>();
            _shell = new Lazy<ShellViewModel>(shell);
            _containerContext = containerContext;
            _eventHub = eventHub;
        }

        public INavigationRequest<TModel> Navigate<TModel>() where TModel : class, IScreen
        {
            return new NavigationRequest<TModel>(this);
        }

        internal TViewModel Resolve<TViewModel>(NavigationRequest<TViewModel> request) where TViewModel : class, IScreen
        {
            var lifetime = _containerContext.BeginLifetimeScope();
            var vm = lifetime.Resolve<TViewModel>();
            request.SetLifetime(lifetime);
            return vm;
        }

        internal async Task Go<TViewModel>(TViewModel model, NavigationRequest<TViewModel> request, bool endPrevious) where TViewModel : class, IScreen
        {

            if (endPrevious && _navigationStack.Count > 0)
            {
                INavigationRequest previous;
                lock (_locker)
                {
                    previous = _navigationStack.Peek();
                }
                await Execute.OnUIThreadAsync(() => _shell.Value.ActivateItem(model));

                await previous.Complete();
                previous.Dispose();
            }
            else
            {
                Execute.BeginOnUIThread(() => _shell.Value.ActivateItem(model));
            }

            lock (_locker)
            {
                _navigationStack.Push(request);
            }
        }

        internal void IssueSignal(INavigationRequest request, NavigationStatus status)
        {
            _eventHub.PublishOnUIThread(new NavigationSignal(request, status));
        }

        internal void Complete<TViewModel>(NavigationRequest<TViewModel> completedRequest) where TViewModel : class, IScreen
        {
            var current = _navigationStack.Peek();
            if (current != completedRequest)
                throw new InvalidOperationException("Child screens are active. This request cannot be ended before child requested are completed");

            _navigationStack.Pop();
        }
    }

    public static class NavigationExtensions
    {
        public static INavigationRequest<TViewModel> ExpectOutcome<TViewModel, TOutcome>(this INavigationRequest<TViewModel> request, IExpectResult<TOutcome> handler) where TViewModel : class,IScreen, IHaveOutcome<TOutcome>
        {
            var actualRequest = request as NavigationRequest<TViewModel>;
            if (actualRequest != null)
            {
                actualRequest.RegisterPostprocessor(x =>
                {
                    var result = x.GetResult();
                    handler.AcceptResult(result);
                });
            }

            return request;
        }


        public static INavigationRequest<TViewModel> With<TViewModel, TArg>(this INavigationRequest<TViewModel> request, TArg args) where TViewModel : class,IScreen, IHasArgument<TArg>
        {
            var actualRequest = request as NavigationRequest<TViewModel>;
            if (actualRequest != null)
            {
                actualRequest.RegisterPreprocessor(x =>
                {
                    x.AcceptArgument(args);
                });
            }

            return request;
        }
    }
}
