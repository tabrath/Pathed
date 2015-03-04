using Pathed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathed.Services
{
    public interface IPathHistoryService
    {
        IEnumerable<PathHistoryEntry> GetEntries();
        void ClearEntries();
        void ClearEntriesAfter(PathHistoryEntry entry);
        PathHistoryEntry Add(string path, EnvironmentVariableTarget target);
    }
}
