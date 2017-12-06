using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModuleSystem.Common.Common;
using ModuleSystem.Common.NavigationInterface;
using Caliburn.Micro;
using ModuleSystem.ModuleAutofac.Common;

namespace ModuleSystem.ModuleAutofac.Login.ViewModels
{
    public class LoginViewModel:BaseScreen
    {
        private IEventAggregator _eventHub;
        private INavigator _navigator;
        private ApplicationContext _context;

        private string _username;
        private string _password;

        public LoginViewModel(INavigator navigator, IEventAggregator eventHub, ApplicationContext context)
        {
            _navigator = navigator;
            _eventHub = eventHub;
            _context = context;
        }
        /// <summary>
        /// Gets or sets the Password.
        /// </summary>
        public string Password
        {
            get
            {
                return this._password;
            }

            set
            {
                if (this._password != value)
                {
                    this._password = value;

                    this.NotifyOfPropertyChange("Password");
                }
            }
        }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        public string Username
        {
            get
            {
                return this._username;
            }

            set
            {
                if (this._username != value)
                {
                    this._username = value;

                    this.NotifyOfPropertyChange("Username");
                }
            }
        }

        public void Login()
        {
            if (string.IsNullOrWhiteSpace(this._username) || string.IsNullOrWhiteSpace(this._password))
            {
                MessageBox.Show("Username cannot be empty", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                _context.AuthenticationContext.CurrentUser = new UserSnapshot("Joby James", "1",new []{"admin","user"});
                 _navigator.Navigate<Main.ViewModels.MainViewModel>().Go();
               //var user=new UserSnapshot()
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }
    }
}
