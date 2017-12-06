using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleSystem.Common.DialogInterface;
using ModuleSystem.Common.NavigationInterface;
using ModuleSystem.ModuleAutofac.Configurations;
using Caliburn.Micro;

namespace ModuleSystem.ModuleAutofac.Common
{
    [SingleInstance]
    public class ApplicationContext
    {
        /// <summary>
        /// Creates a new application context
        /// </summary>
        /// <param name="dialogProvider">Modal dialog provider</param>
        /// <param name="navigator">navigation provider</param>
        /// <param name="authContext">Authentication context</param>
        public ApplicationContext(IDialogProvider dialogProvider, INavigator navigator, AuthenticationContext authContext)
        {
            AuthenticationContext = authContext;
            this.DialogProvider = dialogProvider;
            this.Navigator = navigator;
        }

        /// <summary>
        /// Current modal dialog provider
        /// </summary>
        public IDialogProvider DialogProvider { get; private set; }

        /// <summary>
        /// Navigator for current shell
        /// </summary>
        public INavigator Navigator { get; private set; }



        public AuthenticationContext AuthenticationContext { get; private set; }
    }

    public class AuthenticationContext
    {
        private Caliburn.Micro.IEventAggregator _eventHub;
        public AuthenticationContext(Caliburn.Micro.IEventAggregator eventHub)
        {
            _eventHub = eventHub;
        }
        public bool IsAuthenticated { get; set; }
        /// <summary>
        /// The Current User.
        /// </summary>
        private UserSnapshot _currentUser;

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        /// <value>The current user.</value>
        public UserSnapshot CurrentUser
        {
            get
            {
                return this._currentUser;
            }

            set
            {
                this._currentUser = value;
                this._eventHub.BeginPublishOnUIThread(new Messages.AuthenticationChangedSignal() { IsAuthenticated = true });
            }
        }
    }

    public class UserSnapshot
    {
        public string Id { get; private set; }
        public string FullName { get; private set; }
        /// <summary>
        /// Gets or sets the user roles.
        /// </summary>
        /// <value>The user roles.</value>
        public string[] Roles
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UserSnapshot"/> class.
        /// </summary>
        public UserSnapshot(string name,string id,IEnumerable<string> roles)
        {
            this.FullName = name;
            this.Id = id;
            this.Roles = roles.ToArray();
        }
    }
}
