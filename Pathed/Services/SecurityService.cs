using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Security.Principal;

namespace Pathed.Services
{
    [Export(typeof(ISecurityService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class SecurityService : ISecurityService
    {
        private readonly IApplicationService applicationService;

        [ImportingConstructor]
        public SecurityService(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void ElevateToAdministrator(params string[] args)
        {
            if (IsAdministrator())
                throw new InvalidOperationException("Already running as administrator.");

            var executable = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo(executable)
            {
                Verb = "runas",
                Arguments = String.Join(" ", args)
            };
            Process.Start(startInfo);

            this.applicationService.Shutdown();
        }
    }
}
