using System;

namespace PilotObjectInfo.Core.DeepAnalytics
{
    internal class AssemblyMetaDataDTO
    {
        internal Guid Id { get; set; }
        internal string InternalName { get; set; }
        internal string Version { get; set; }
        internal string Product { get; set; }
        internal string Company { get; set; }
    }
}
