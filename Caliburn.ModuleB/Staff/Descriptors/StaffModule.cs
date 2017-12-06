using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleSystem.Common.Common;
using ModuleSystem.Common.NavigationInterface;
using ModuleSystem.Common.NavigationMenuInterface;

namespace ModuleSystem.ModuleB.Staff.Descriptors
{
    [Authorize(Roles = new[] { "user" })]
    public class StaffModule:INavMenu
    {
        #region Private Fields

        private INavigator _navigator;

        #endregion Private Fields

        #region Public Constructors

        public StaffModule(INavigator navigator)
        {
            _navigator = navigator;
            Name = "Staff Module";
            Description = "Staff Module Screen";
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
            await _navigator.Navigate<Staff.ViewModels.StaffViewModel>().Go();
        }

        #endregion Public Methods
    }
}
