using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModuleSystem.ModuleAutofac.Shell.ViewModels;

namespace ModuleSystem.ModuleAutofac.Configurations
{
    public class ApplicationBootstrapper:AutofacBootstrapper<ShellViewModel>
    {
        public ApplicationBootstrapper(): base("ModuleSystem.*.dll")
        {
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            base.OnStartup(sender, e);
        }
    }
}
