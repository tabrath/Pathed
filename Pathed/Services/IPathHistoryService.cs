using System;
using System.Collections.Generic;
using Pathed.Models;

namespace Pathed.Services
{
    public interface IPathHistoryService
    {
        IEnumerable<PathHistoryEntry> Entries { get; }
        void ClearEntries();
        void ClearEntriesAfter(PathHistoryEntry entry);
        PathHistoryEntry Add(string path, EnvironmentVariableTarget target);
    }
}
