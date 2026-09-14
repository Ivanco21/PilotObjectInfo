using System;
using System.Collections.Generic;

namespace PilotObjectInfo.Core.DeepAnalytics
{
    internal class AssemblyRawData
    {
        internal Meta Metadata { get; set; } = new Meta();
        internal class Meta
        {
            internal Guid Id { get; set; }
            internal string Company { get; set; }
            internal string Product { get; set; }
            internal string Version { get; set; }
            internal string InternalName { get; set; }
        }
        internal HashSet<string> Strings { get; set; } = new();
    }
}
