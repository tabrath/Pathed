using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathed.Services
{
    public interface IWatchService
    {
        void Watch(string name, EnvironmentVariableTarget target, Action<string, EnvironmentVariableTarget> action = null);
        void Unwatch(string name, EnvironmentVariableTarget target);
        void SetIsSaving(string name, EnvironmentVariableTarget target);
    }
}
