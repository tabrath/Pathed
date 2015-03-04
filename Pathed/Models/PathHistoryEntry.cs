using System;

namespace Pathed.Models
{
    [Serializable]
    public class PathHistoryEntry
    {
        public string Value { get; set; }
        public EnvironmentVariableTarget Target { get; set; }
        public DateTime DateEdited { get; set; }

        public PathHistoryEntry() { }

        public PathHistoryEntry(string value, EnvironmentVariableTarget target)
        {
            Value = value;
            Target = target;
            DateEdited = DateTime.Now;
        }
    }
}
