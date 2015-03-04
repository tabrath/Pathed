using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pathed.Services
{
    [Export(typeof(IWatchService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class WatchService : IWatchService, IPartImportsSatisfiedNotification, IDisposable
    {
        [Import(typeof(IEnvironmentService))]
        private IEnvironmentService environmentService;

        private List<Watcher> watchers;

        [ImportingConstructor]
        public WatchService()
        {
            this.watchers = new List<Watcher>();
        }

        ~WatchService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var watcher in watchers)
                {
                    lock (watcher.SyncLock)
                    {
                        watcher.Timer.Dispose();
                    }
                }
            }
        }

        public void Watch(string name, EnvironmentVariableTarget target, Action<string, EnvironmentVariableTarget> action = null)
        {
            Watcher watcher;
            if (TryGetWatcher(name, target, out watcher))
                throw new InvalidOperationException("Environment variable already watched.");

            watcher = new Watcher() { Name = name, Target = target, Action = action, IsSaving = false, LastValue = null };

            lock (watcher.SyncLock)
            {
                this.watchers.Add(watcher);

                watcher.Timer = new Timer(async (x) => await WatcherCallback(x), watcher, 0, 1000);
            }
        }

        private async Task WatcherCallback(object state)
        {
            Watcher watcher = (Watcher)state;

            var value = await this.environmentService.GetVariableAsync(watcher.Name, watcher.Target);

            lock (watcher.SyncLock)
            {
                if (watcher.LastValue == null)
                {
                    watcher.LastValue = value;
                }
                else
                {
                    if (watcher.LastValue != value)
                    {
                        watcher.LastValue = value;

                        if (!watcher.IsSaving)
                        {
                            watcher.Action(value, watcher.Target);
                        }
                        else
                        {
                            watcher.IsSaving = false;
                        }
                    }
                }
            }
        }

        public void Unwatch(string name, EnvironmentVariableTarget target)
        {
            Watcher watcher;
            if (TryGetWatcher(name, target, out watcher))
            {
                lock (watcher.SyncLock)
                {
                    watcher.Timer.Dispose();

                    this.watchers.Remove(watcher);
                }
            }
        }

        public void SetIsSaving(string name, EnvironmentVariableTarget target)
        {
            Watcher watcher;
            if (TryGetWatcher(name, target, out watcher))
            {
                lock (watcher.SyncLock)
                {
                    watcher.IsSaving = true;
                }
            }
        }

        private bool TryGetWatcher(string name, EnvironmentVariableTarget target, out Watcher watcher)
        {
            try
            {
                watcher = this.watchers.Single(x => x.Name.Equals(name) && x.Target.Equals(target));
                return (watcher != null);
            }
            catch
            {
                watcher = null;
                return false;
            }
        }

        public void OnImportsSatisfied()
        {
        }

        private class Watcher
        {
            public string Name { get; set; }
            public EnvironmentVariableTarget Target { get; set; }
            public bool IsSaving { get; set; }
            public Timer Timer { get; set; }
            public string LastValue { get; set; }
            public Action<string, EnvironmentVariableTarget> Action { get; set; }
            public Object SyncLock { get; set; }

            public Watcher()
            {
                SyncLock = new Object();
            }
        }
    }
}
