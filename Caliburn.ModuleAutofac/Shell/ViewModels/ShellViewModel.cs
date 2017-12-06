using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModuleSystem.Common.NavigationMenuInterface;
using Caliburn.Micro;
using ModuleSystem.ModuleAutofac.Common;
using ModuleSystem.ModuleAutofac.Configurations;
using ModuleSystem.ModuleAutofac.Navigation;
using ModuleSystem.ModuleAutofac.Messages;
using ModuleSystem.Common.Common;

namespace ModuleSystem.ModuleAutofac.Shell.ViewModels
{
    [SingleInstance]
    public class ShellViewModel : Conductor<IScreen>, IHandle<NavigationSignal>, IHandle<AuthenticationChangedSignal>
    {
        private ApplicationContext _context;

        private IEventAggregator _eventHub;

        public ShellViewModel(ApplicationContext context, IEventAggregator eventHub, IEnumerable<INavMenu> modules)
        {
            this._isBusy = new Stack<bool>();
            this._modules = modules;
            _eventHub = eventHub;
            _eventHub.Subscribe(this);
            _context = context;
            DisplayName = "Hello World";
        }

        private IEnumerable<INavMenu> _filterModules;

        private IEnumerable<INavMenu> _modules;

        public IEnumerable<INavMenu> FilterModules
        {
            get
            {
                return this._filterModules;
            }

            set
            {
                this._filterModules = value;
                this.NotifyOfPropertyChange(() => this.FilterModules);
            }
        }

        private bool isAuthenticated;

        public bool IsAuthenticated
        {
            get
            {
                return this.isAuthenticated;
            }
            set
            {
                this.isAuthenticated = value;
                this.NotifyOfPropertyChange(() => this.IsAuthenticated);
            }
        }

        private Stack<bool> _isBusy;

        public bool IsBusy
        {
            get
            {
                return this._isBusy.Count > 0;
            }
            set
            {
                if (value)
                    this._isBusy.Push(value);
                else if (this._isBusy.Count > 0)
                    this._isBusy.Pop();

                this.NotifyOfPropertyChange(() => this.IsBusy);
            }
        }

        public override void ActivateItem(IScreen item)
        {
            base.ActivateItem(item);
        }

        public void Handle(AuthenticationChangedSignal message)
        {
            var current = _context.AuthenticationContext.CurrentUser;
            IsAuthenticated = message.IsAuthenticated && current != null;
            if (IsAuthenticated)
            {
                FilterModules = _modules.Where(x =>
                {
                
                    var attrs = x.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), false).Cast<AuthorizeAttribute>().FirstOrDefault();
                    return attrs == null || attrs.Roles.Any(r => current.Roles.Any(cr => cr == r));

                }).ToList();
            }
            else
                _filterModules = Enumerable.Empty<INavMenu>();
        }

        public void Handle(NavigationSignal message)
        {
            if (message.Status == NavigationStatus.Begin)
            {
                this.IsBusy = true;
            }
            else
            {
                this.IsBusy = false;
            }
        }

        protected override void OnActivate()
        {

            base.OnActivate();
            if (ActiveItem == null)
                _context.Navigator.Navigate<Login.ViewModels.LoginViewModel>().Go();
        }

    }
}
