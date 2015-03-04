using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pathed.Services
{
    [Export(typeof(IApplicationService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ApplicationService : IApplicationService, IPartImportsSatisfiedNotification
    {
        [Import(typeof(ISettingsService))]
        private ISettingsService settingsService;

        [ImportingConstructor]
        public ApplicationService()
        {
        }

        public void Shutdown()
        {
            Application.Current.Shutdown();
        }

        public string GetTitle()
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        }

        public Version GetVersion()
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetName().Version;
        }

        public void OnImportsSatisfied()
        {
            Application.Current.Exit += (s, e) => { if (e.ApplicationExitCode == 0) this.settingsService.Save(); };
        }
    }
}
