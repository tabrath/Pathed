using Pathed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathed.Services
{
    [Export(typeof(IPathHistoryService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PathHistoryService : IPathHistoryService
    {
        [Import(typeof(ISettingsService))]
        private ISettingsService settingsService;

        [ImportingConstructor]
        public PathHistoryService() { }

        public IEnumerable<PathHistoryEntry> GetEntries()
        {
            return this.settingsService.Get<PathHistoryEntry[]>("PathHistoryEntries", new PathHistoryEntry[] { });
        }

        public void ClearEntries()
        {
            this.settingsService.Set<PathHistoryEntry[]>("PathHistoryEntries", new PathHistoryEntry[] { });
        }

        public void ClearEntriesAfter(PathHistoryEntry entry)
        {
            var entries = GetEntries().ToList();
            var index = entries.IndexOf(entry);
            entries.RemoveRange(index, entries.Count - index);
            this.settingsService.Set<PathHistoryEntry[]>("PathHistoryEntries", entries.ToArray());
        }

        public PathHistoryEntry Add(string path, EnvironmentVariableTarget target)
        {
            var entries = GetEntries().ToList();
            var entry = new PathHistoryEntry(path, target);
            entries.Add(entry);
            this.settingsService.Set<PathHistoryEntry[]>("PathHistoryEntries", entries.ToArray());
            return entry;
        }
    }
}
