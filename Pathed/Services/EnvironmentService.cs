using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Pathed.Services
{
    [Export(typeof(IEnvironmentService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class EnvironmentService : IEnvironmentService
    {
        public Task<IEnumerable<string>> GetPathsAsync(EnvironmentVariableTarget target)
        {
            return Task.Factory.StartNew<IEnumerable<string>>(() =>
            {
                return Environment.GetEnvironmentVariable("PATH", target).Split(';').OrderBy(x => x).ToArray();
            });
        }

        public Task SetPathsAsync(IEnumerable<string> paths, EnvironmentVariableTarget target)
        {
            return Task.Factory.StartNew(() =>
            {
                Environment.SetEnvironmentVariable("PATH", string.Join(";", paths.OrderBy(x => x).ToArray()), target);
            });
        }

        public Task<string> GetVariableAsync(string name, EnvironmentVariableTarget target)
        {
            return Task.Run<string>(() =>
                {
                    return Environment.GetEnvironmentVariable(name, target);
                });
        }

        public Task SetVariableAsync(string name, string value, EnvironmentVariableTarget target)
        {
            return Task.Run(() =>
                {
                    Environment.SetEnvironmentVariable(name, value, target);
                });
        }
    }
}
