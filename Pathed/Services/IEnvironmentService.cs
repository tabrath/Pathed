using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathed.Services
{
    public interface IEnvironmentService
    {
        Task<string> GetVariableAsync(string name, EnvironmentVariableTarget target);
        Task SetVariableAsync(string name, string value, EnvironmentVariableTarget target);
        Task<IEnumerable<string>> GetPathsAsync(EnvironmentVariableTarget target);
        Task SetPathsAsync(IEnumerable<string> paths, EnvironmentVariableTarget target);
    }
}
