using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Pathed.Models;

namespace Pathed.Services
{
    [Export(typeof(IPathHistoryService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PathHistoryService : IPathHistoryService
    {
        [Import(typeof(ISettingsService))]
        private ISettingsService settingsService;

        public IEnumerable<PathHistoryEntry> Entries
        {
            get { return this.settingsService.Get<PathHistoryEntry[]>("PathHistoryEntries", new PathHistoryEntry[] { }); }
        }

        [ImportingConstructor]
        public PathHistoryService() { }

        public void ClearEntries()
        {
            this.settingsService.Set<PathHistoryEntry[]>("PathHistoryEntries", new PathHistoryEntry[] { });
        }

        public void ClearEntriesAfter(PathHistoryEntry entry)
        {
            var entries = Entries.ToList();
            var index = entries.IndexOf(entry);
            entries.RemoveRange(index, entries.Count - index);
            this.settingsService.Set<PathHistoryEntry[]>("PathHistoryEntries", entries.ToArray());
        }

        public PathHistoryEntry Add(string path, EnvironmentVariableTarget target)
        {
            var entries = Entries.ToList();
            var entry = new PathHistoryEntry(path, target);
            entries.Add(entry);
            this.settingsService.Set<PathHistoryEntry[]>("PathHistoryEntries", entries.ToArray());
            return entry;
        }
    }
}
