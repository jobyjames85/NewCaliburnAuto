using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleSystem.Common.Common;
using ModuleSystem.Common.NavigationInterface;
using ModuleSystem.Common.NavigationMenuInterface;

namespace ModuleSystem.ModuleA.Admin.Descriptors
{
    [Authorize(Roles = new[] { "admin" })]
    public class AdminModule : INavMenu
    {
        #region Private Fields

        private INavigator _navigator;

        #endregion Private Fields

        #region Public Constructors

        public AdminModule(INavigator navigator)
        {
            _navigator = navigator;
            Name = "Admin Module";
            Description = "Admin Module Screen";
        }

        #endregion Public Constructors

        #region Public Properties

        public string Description
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        public async Task Activate()
        {
            await _navigator.Navigate<Admin.ViewModels.AdminViewModel>().Go();
        }

        #endregion Public Methods
    }
}
