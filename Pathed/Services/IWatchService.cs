using System;

namespace Pathed.Services
{
    public interface IWatchService
    {
        void Watch(string name, EnvironmentVariableTarget target, Action<string, EnvironmentVariableTarget> action);
        void Unwatch(string name, EnvironmentVariableTarget target);
        void SetIsSaving(string name, EnvironmentVariableTarget target);
    }
}
