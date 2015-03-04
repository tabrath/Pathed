using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows;

namespace Pathed.Services
{
    [Export(typeof(IApplicationService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ApplicationService : IApplicationService, IPartImportsSatisfiedNotification
    {
        [Import(typeof(ISettingsService))]
        private ISettingsService settingsService;

        private string title;
        public string Title
        {
            get
            {
                if (this.title == null)
                    this.title = GetTitle();

                return this.title;
            }
        }

        private Version version;
        public Version Version
        {
            get
            {
                if (this.version == null)
                    this.version = GetVersion();

                return this.version;
            }
        }

        [ImportingConstructor]
        public ApplicationService()
        {
        }

        public void Shutdown()
        {
            Application.Current.Shutdown();
        }

        private static string GetTitle()
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        }

        private static Version GetVersion()
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
